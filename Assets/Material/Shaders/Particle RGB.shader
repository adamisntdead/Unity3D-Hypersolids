Shader "Particles/RGB Cube" { 
	Properties {
		alpha ("Alpha", Range(0,1)) = 0.1
		mult ("Brightness", Range(-1,1)) = 0
		cons ("Contrast", Range(-1,1)) = 0
	}
   SubShader { 
   Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
   Blend SrcAlpha One
	AlphaTest Greater .01
	ColorMask RGB
	Cull Off Lighting Off ZWrite Off
	
      Pass { 
         CGPROGRAM 
 
         #pragma vertex vert // vert function is the vertex shader 
         #pragma fragment frag // frag function is the fragment shader
 uniform half alpha;
 uniform half cons;
 uniform half mult;
         // for multiple vertex output parameters an output structure 
         // is defined:
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 col : TEXCOORD0;
         };
 
         vertexOutput vert(float4 vertexPos : POSITION) 
            // vertex shader 
         {
            vertexOutput output; // we don't need to type 'struct' here
 
 			half a = 1 + cons;
			half b = mult - cons * 0.5f;  
 
            output.pos =  mul(UNITY_MATRIX_MVP, vertexPos);
            output.col = half4( (normalize(vertexPos.xyz 
            		+ half3(0.5, 0.5, 0.5))) *a+b,alpha); 
                // Here the vertex shader writes output data
               // to the output structure. We add 0.5 to the 
               // x, y, and z coordinates, because the 
               // coordinates of the cube are between -0.5 and
               // 0.5 but we need them between 0.0 and 1.0. 
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR // fragment shader
         {
              return input.col; 
               // Here the fragment shader returns the "col" input 
               // parameter with semantic TEXCOORD0 as nameless
               // output parameter with semantic COLOR.
         }
 
         ENDCG  
      }
   }
}