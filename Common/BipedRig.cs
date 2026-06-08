using Microsoft.Xna.Framework;
namespace TheBrokenScript.Common;
public struct BipedRig
{
	public struct BoneRootStr
	{
		public Vector2 BaseHeadRoot;
		public Vector2 BaseTorsoRoot;
		public Vector2 BaseLeftArmRoot;
		public Vector2 BaseRightArmRoot;
		public Vector2 BaseLeftLegRoot;
		public Vector2 BaseRightLegRoot;
	}
	public struct BoneLeafStr
	{
		public Vector2 BaseHeadTarget;
		public Vector2 BaseTorsoTarget;
		public Vector2 BaseLeftArmTarget;
		public Vector2 BaseRightArmTarget;
		public Vector2 BaseLeftLegTarget;
		public Vector2 BaseRightLegTarget;
	}
	public struct BoneLengthStr
	{
		public float HeadBoneLength;
		public float TorsoBoneLength;
		public float LeftUpperArmLength;
		public float LeftLowerArmLength;
		public float RightUpperArmLength;
		public float RightLowerArmLength;
		public float LeftUpperLegLength;
		public float LeftLowerLegLength;
		public float RightUpperLegLength;
		public float RightLowerLegLength;
	}
	public BoneRootStr BoneRoot;
	public BoneLeafStr BoneLeaf;
	public BoneLengthStr BoneLength;
	public FABRIK[] BoneChains;
}