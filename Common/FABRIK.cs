using Microsoft.Xna.Framework;
using System;
namespace TheBrokenScript.Common;
public class FABRIK
{
	public Vector2[] Joints; // Positions of each joint
	public float[] Bones; // Length of each bone [Joints[i] -> Joints[i-1]
	public int BoneCount => Joints.Length - 1;
	public Vector2? PoleTarget = null; // Null to disable, affects middle joints
	public float PoleTargetStrength = 0.5f;
	// Create an IK Chain from root downwards
	public FABRIK(Vector2 rootJoint, int boneCount, float[] boneLengths)
	{
		if (boneLengths.Length != boneCount)
		{
			throw new ArgumentException("boneLengths must have exactly boneCount elements.");
		}
		Joints = new Vector2[boneCount + 1];
		Bones = new float[boneCount];
		Joints[0] = rootJoint;
		for (int i = 0; i < boneCount; i++)
		{
			Joints[i + 1] = Joints[i] + new Vector2(0, boneLengths[i]);
			Bones[i] = boneLengths[i];
		}
	}
	public void Rebase(Vector2 newRoot)
	{
		Vector2 delta = newRoot - Joints[0];
		for (int i = 0; i < Joints.Length; i++)
		{
			Joints[i] += delta;
		}
	}
	// Solve the IK Chain twice, once from tip to root then from root to tip to make sure the root is fixed to the body
	public void Solve(Vector2 root, Vector2 targetLocation, int iterations = 10)
	{
		float totalLength = 0;
		foreach (float length in Bones)
		{
			totalLength += length;
		}
		// If the target is unreachable, stretch (clamping totalLength won't work as I think btw)
		if (Vector2.Distance(root, targetLocation) >= totalLength)
		{
			StretchToward(root, targetLocation);
			return;
		}
		// Forward and Backward Passes
		for (int i = 0; i < iterations; i++)
		{
			// Forward: Tip to Target and then go through the joints to the root
			// ^1 = last element/joint >.>
			Joints[^1] = targetLocation;
			for (int a = BoneCount - 1; a >= 0; a--)
			{
				Vector2 direction = Vector2.Normalize(Joints[a] - Joints[a + 1]);
				Joints[a] = Joints[a + 1] + (direction * Bones[a]);
			}
			// Backward: Anchor root and then go through the joints to the tip
			Joints[0] = root;
			for (int a = 0; a < BoneCount; a++)
			{
				Vector2 direction = Vector2.Normalize(Joints[a + 1] - Joints[a]);
				Joints[a + 1] = Joints[a] + (direction * Bones[a]);
			}
		}
		// Pole Target Pass
		if (PoleTarget.HasValue)
		{
			ApplyPoleTarget();
		}
		ApplyPoleTarget2Bone();
	}
	private void ApplyPoleTarget() //For each interior joint, project on the root->tip line and blend towards PoleTarget while preserving Bone Lengths.
	{
		Vector2 pole = PoleTarget.Value;
		for (int i = 1; i < Joints.Length - 1; i++) //Skip Root and Tip Joints
		{
			Vector2 prevJoint = Joints[i - 1];
			Vector2 nextJoint = Joints[i + 1];
			// Closest Point on the bone between prevJoint and nextJoint to this Joint
			Vector2 lineDirection = nextJoint - prevJoint;
			float lineLength = lineDirection.Length();
			if (lineLength < 0.001f) // if Joints are on top of eachother.
			{
				continue;
			}
			lineDirection /= lineLength; // Normalize to unit vector
										 // Project joint onto the line, see how far it is on it
			float distanceAlongLine = Vector2.Dot(Joints[i] - prevJoint, lineDirection);
			Vector2 closestPointOnLine = prevJoint + (lineDirection * distanceAlongLine);
			// Find direction from point to PoleTarget
			Vector2 directionToPole = pole - closestPointOnLine;
			float distanceToPole = directionToPole.Length();
			if (distanceToPole < 0.001f) { continue; }
			directionToPole /= distanceToPole; // normalize to unit vector
											   // Push "knee" towards the pole by PoleTargetStrength multiplier
			Vector2 poleInfluencedPosition = closestPointOnLine + directionToPole * PoleTargetStrength * Bones[i - 1];
			// Reconstrain bone lengths after the PoleTarget is applied
			Vector2 directionFromPrev = Vector2.Normalize(poleInfluencedPosition - prevJoint);
			Joints[i] = prevJoint + directionFromPrev * Bones[i - 1];
			// Fix next joint as well
			Vector2 directionFromKnee = Vector2.Normalize(nextJoint - Joints[i]);
			Joints[i + 1] = Joints[i] + directionFromKnee * Bones[i];
			// I need to become better at math ngl
		}
	}
	private void ApplyPoleTarget2Bone()
	{
		if (!PoleTarget.HasValue || BoneCount != 2) return;

		Vector2 root = Joints[0];
		Vector2 tip = Joints[2];
		Vector2 pole = PoleTarget.Value;

		float a = Bones[0]; // upper bone
		float b = Bones[1]; // lower bone
		float dist = Vector2.Distance(root, tip);
		if (dist < 0.001f) return;

		// Law of cosines to find the angle at the root
		float cosAngle = (a * a + dist * dist - b * b) / (2 * a * dist);
		cosAngle = Math.Clamp(cosAngle, -1f, 1f);
		float angle = MathF.Acos(cosAngle);

		// Project pole onto the plane perpendicular to root->tip
		Vector2 rootToTip = Vector2.Normalize(tip - root);
		Vector2 rootToPole = pole - root;
		float proj = Vector2.Dot(rootToPole, rootToTip);
		Vector2 polePerp = rootToPole - rootToTip * proj;
		float perpLen = polePerp.Length();
		if (perpLen < 0.001f) return;
		Vector2 bendDir = polePerp / perpLen;

		// Place knee using the angle and bend direction
		Joints[1] = root + (rootToTip * MathF.Cos(angle) + bendDir * MathF.Sin(angle)) * a;
		// Reconstrain tip
		Vector2 kneeToTip = Vector2.Normalize(tip - Joints[1]);
		Joints[2] = Joints[1] + kneeToTip * b;
	}
	private void StretchToward(Vector2 root, Vector2 target)
	{
		Vector2 direction = Vector2.Normalize(target - root);
		Joints[0] = root;
		for (int i = 1; i <= BoneCount; i++)
		{
			Joints[i] = Joints[i - 1] + (direction * Bones[i - 1]);
		}
	}
}
