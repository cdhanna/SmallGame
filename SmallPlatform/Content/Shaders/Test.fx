
sampler TextureSampler : register(s0);

float4x4 World;
float4x4 View;
float4x4 Projection;

sampler BaseSampler : register(s1)
{
	Texture = (ParticleData);
	Filter = Linear;
	AddressU = clamp;
	AddressV = clamp;
};

struct VertexShaderInput
{
	float4 Color: COLOR0;
	float4 Position : SV_POSITION;
};

struct VertexShaderOutput
{
	float4 Color: COLOR0;
	float4 Position : SV_POSITION;
};


float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // Look up the original image color.
	//float4 c = tex2D(TextureSampler, texCoord);
	float4 c = input.Color;
	// Adjust it to keep only values brighter than the specified threshold.
	return c;
}

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;
	output.Color = input.Color;
	
	float4 pos = input.Position;

		// fetch actual position from texture. 
		//float4 particlePos = ParticleData.SampleLevel(BaseSampler, float2(pos.x, pos.y), 0);
		float4 particlePos = tex2Dlod(BaseSampler, float4(pos.x, pos.y, 0, 0));
		pos.x = particlePos.x;
	pos.y = particlePos.y;

	// extend out into a triangle.
	if (pos.z == 0){
		pos.x -= 10;
		pos.y -= 10;
	}
	else if (pos.z == 1){
		pos.x += 10;
		pos.y -= 10;
	}
	else if (pos.z == 2){
		pos.y += 10;
	}
	pos.z = 0;


	float4 worldPosition = mul(pos, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);


	//if (output.Position.z == 0){
	//	output.Position.x -= 1;
	//	output.Position.y -= 1;
	//}

	//if (output.Position.z == 1){
	//	output.Position.x += 1;
	//	output.Position.y -= 1;
	//}

	//if (output.Position.z == 2){
	//	output.Position.y += 1;
	//}
	//output.Position.x = output.Position.x + 1;

	return output;
}


technique TestTech
{
    pass Pass1
    {
        #if SM4

		VertexShader = compile vs_4_0_level_9_1 VertexShaderFunction();
		PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
			
        #elif SM3

		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction();

        #else

		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction();
		
        #endif

    }
}