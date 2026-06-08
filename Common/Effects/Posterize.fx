sampler uImage0 : register(s0);
float uPosterizeLevels : register(c0);
float2 uImageSize0 : register(c1);
float4 main(float2 uv : TEXCOORD0) : COLOR0
{
	float4 color = tex2D(uImage0, uv);
	color.rgb = floor(color.rgb * uPosterizeLevels) / uPosterizeLevels;
	return color;
}
technique Posterize
{
	pass One
	{
		PixelShader = compile ps_3_0 main();
	}
}