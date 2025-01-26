// Made with Amplify Shader Editor v1.9.2.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SimpleLiquid_Translated"
{
	Properties
	{
		[HideInInspector]_WobbleX("Wobble X", Float) = 0
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		[HideInInspector]_WobbleY("Wobble Y", Float) = 0
		[HideInInspector]_WobbleZ("Wobble Z", Float) = 0
		_BeverageColor("Beverage Color", Color) = (1,0.5817639,0,1)
		_FoamPercentage("Foam Percentage", Range( 0 , 100)) = 15
		_FoamColor("Foam Color", Color) = (1,1,1,1)
		_FoamColor1("Foam Color", Color) = (1,1,1,1)
		[HideInInspector]_Fill_Amount("Fill Amount", Range( 0 , 1)) = 0.5
		[HideInInspector]_Fill_Amount1("Fill Amount", Range( 0 , 1)) = 0.5
		[HideInInspector]_Glass_Top_Distance("Glass Top Distance", Range( 0 , 1)) = 0.284
		[HideInInspector]_GlassBottom("Glass Bottom", Vector) = (0,0,0,0)
		[HideInInspector]_GlassTop("Glass Top", Vector) = (0,0,0,0)
		_WobbleIntensity("Wobble Intensity", Float) = 0.15
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows 
		struct Input
		{
			half ASEIsFrontFacing : VFACE;
			float3 worldPos;
		};

		uniform float4 _FoamColor;
		uniform float4 _BeverageColor;
		uniform float4 _FoamColor1;
		uniform float _WobbleIntensity;
		uniform float _WobbleX;
		uniform float _Fill_Amount1;
		uniform float3 _GlassTop;
		uniform float _WobbleY;
		uniform float _WobbleZ;
		uniform float3 _GlassBottom;
		uniform float _FoamPercentage;
		uniform float _Fill_Amount;
		uniform float _Glass_Top_Distance;
		uniform float _Cutoff = 0.5;


		float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
		{
			original -= center;
			float C = cos( angle );
			float S = sin( angle );
			float t = 1 - C;
			float m00 = t * u.x * u.x + C;
			float m01 = t * u.x * u.y - S * u.z;
			float m02 = t * u.x * u.z + S * u.y;
			float m10 = t * u.x * u.y + S * u.z;
			float m11 = t * u.y * u.y + C;
			float m12 = t * u.y * u.z - S * u.x;
			float m20 = t * u.x * u.z - S * u.y;
			float m21 = t * u.y * u.z + S * u.x;
			float m22 = t * u.z * u.z + C;
			float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
			return mul( finalMatrix, original ) + center;
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 switchResult69 = (((i.ASEIsFrontFacing>0)?(_BeverageColor):(_FoamColor1)));
			float3 ase_worldPos = i.worldPos;
			float3 worldToObj8 = mul( unity_WorldToObject, float4( ase_worldPos, 1 ) ).xyz;
			float3 temp_output_78_0 = ( ( _Fill_Amount1 * ( _GlassTop - worldToObj8 ) ) + worldToObj8 );
			float3 rotatedValue9 = RotateAroundAxis( temp_output_78_0, worldToObj8, float3( 1,0,0 ), 90.0 );
			float3 rotatedValue15 = RotateAroundAxis( temp_output_78_0, worldToObj8, float3( 0,1,0 ), 90.0 );
			float3 rotatedValue22 = RotateAroundAxis( temp_output_78_0, worldToObj8, float3( 0,0,1 ), 90.0 );
			float temp_output_89_0 = abs( _Glass_Top_Distance );
			float4 temp_output_1_0_g1 = ( switchResult69 * step( ( ( ( ( ( _WobbleIntensity * _WobbleX ) * rotatedValue9 ) + ( ( ( _WobbleIntensity * _WobbleY ) * rotatedValue15 ) * 0.1 ) ) + ( ( _WobbleIntensity * _WobbleZ ) * rotatedValue22 ) ) + ( ase_worldPos - _GlassBottom ) ).y , (0.0 + (( ( ( 100.0 - _FoamPercentage ) / 100.0 ) * _Fill_Amount ) - 0.0) * (temp_output_89_0 - 0.0) / (1.0 - 0.0)) ) );
			float4 color51 = IsGammaSpace() ? float4(0,0,0,0) : float4(0,0,0,0);
			float4 temp_output_2_0_g1 = color51;
			float temp_output_11_0_g1 = distance( temp_output_1_0_g1 , temp_output_2_0_g1 );
			float4 lerpResult21_g1 = lerp( _FoamColor , temp_output_1_0_g1 , saturate( ( ( temp_output_11_0_g1 - 0.0 ) / max( 0.0 , 1E-05 ) ) ));
			float temp_output_25_0 = step( ( ( ( ( ( _WobbleIntensity * _WobbleX ) * rotatedValue9 ) + ( ( ( _WobbleIntensity * _WobbleY ) * rotatedValue15 ) * 0.1 ) ) + ( ( _WobbleIntensity * _WobbleZ ) * rotatedValue22 ) ) + ( ase_worldPos - _GlassBottom ) ).y , (0.0 + (_Fill_Amount - 0.0) * (temp_output_89_0 - 0.0) / (1.0 - 0.0)) );
			o.Emission = ( lerpResult21_g1 * temp_output_25_0 ).rgb;
			o.Alpha = 1;
			clip( temp_output_25_0 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19202
Node;AmplifyShaderEditor.CommentaryNode;23;-3715.337,-696.1297;Inherit;False;2724.362;798.7785;;24;82;81;80;11;14;22;15;9;7;8;72;74;71;73;78;12;13;18;16;21;20;19;17;79;Liquid Wobble;0.9937106,0.3093626,0.3093626,1;0;0
Node;AmplifyShaderEditor.BreakToComponentsNode;24;-512,128;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleAddOpNode;4;-768,128;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-896,-384;Inherit;False;Property;_FoamPercentage;Foam Percentage;5;0;Create;True;0;0;0;False;0;False;15;15;0;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;32;-592,-416;Inherit;False;2;0;FLOAT;100;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;33;-384,-416;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;100;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-192,-416;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;35;0,-416;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;30;0,-160;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;25;256,128;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;37;144,48;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;51;640,-416;Inherit;False;Constant;_Color3;Color 3;10;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;50;959.2667,-532.3334;Inherit;True;Replace Color;-1;;1;896dccb3016c847439def376a728b869;1,12,0;5;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldPosInputsNode;1;-1360,160;Inherit;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.StepOpNode;36;256,-416;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;53;704,-32;Inherit;False;Constant;_Float3;Float 3;10;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;1232,-160;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector3Node;62;-1312,384;Inherit;False;Property;_GlassBottom;Glass Bottom;11;1;[HideInInspector];Create;True;0;0;0;False;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ColorNode;64;640,-224;Inherit;False;Property;_FoamColor;Foam Color;6;0;Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SwitchByFaceNode;69;256,-736;Inherit;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;39;-32,-832;Inherit;False;Property;_BeverageColor;Beverage Color;4;0;Create;True;0;0;0;False;0;False;1,0.5817639,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;68;-32,-640;Inherit;False;Property;_FoamColor1;Foam Color;7;0;Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;512,-640;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1498.169,-170.383;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;SimpleLiquid_Translated;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;;0;False;;False;0;False;;0;False;;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.RangedFloatNode;26;-512,-193.0998;Inherit;False;Property;_Fill_Amount;Fill Amount;8;1;[HideInInspector];Create;False;0;0;0;False;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;78;-2544,-336;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector3Node;73;-3296,-544;Inherit;False;Property;_GlassTop;Glass Top;12;1;[HideInInspector];Create;True;0;0;0;False;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;71;-3072,-528;Inherit;False;Property;_Fill_Amount1;Fill Amount;9;1;[HideInInspector];Create;False;0;0;0;False;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;74;-3072,-432;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;-2752,-464;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TransformPositionNode;8;-3328,-256;Inherit;False;World;Object;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;7;-3552,-256;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;17;-1648,-192;Inherit;False;Constant;_Float0;Float 0;3;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-1456,-272;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;81;-1840,-416;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-1664,-320;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;82;-1824,-112;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-1664,-48;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-1664,-528;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;80;-1840,-624;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;20;-1136,-80;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;19;-1280,-384;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;79;-2304,-640;Inherit;False;Property;_WobbleIntensity;Wobble Intensity;13;0;Create;True;0;0;0;False;0;False;0.15;0.15;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-2032,-592;Inherit;False;Property;_WobbleX;Wobble X;0;1;[HideInInspector];Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-2016,-336;Inherit;False;Property;_WobbleY;Wobble Y;2;1;[HideInInspector];Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-2016,-112;Inherit;False;Property;_WobbleZ;Wobble Z;3;1;[HideInInspector];Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RotateAboutAxisNode;9;-2336,-528;Inherit;False;False;4;0;FLOAT3;1,0,0;False;1;FLOAT;90;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RotateAboutAxisNode;15;-2336,-304.6667;Inherit;False;False;4;0;FLOAT3;0,1,0;False;1;FLOAT;90;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RotateAboutAxisNode;22;-2336,-64;Inherit;False;False;4;0;FLOAT3;0,0,1;False;1;FLOAT;90;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;3;-1093.68,261.2;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-647.2001,-60.87999;Inherit;False;Property;_Glass_Top_Distance;Glass Top Distance;10;1;[HideInInspector];Create;False;0;0;0;False;0;False;0.284;0.284;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;89;-234.2399,-104.8086;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
WireConnection;24;0;4;0
WireConnection;4;0;20;0
WireConnection;4;1;3;0
WireConnection;32;1;31;0
WireConnection;33;0;32;0
WireConnection;34;0;33;0
WireConnection;34;1;26;0
WireConnection;35;0;34;0
WireConnection;35;4;89;0
WireConnection;30;0;26;0
WireConnection;30;4;89;0
WireConnection;25;0;24;1
WireConnection;25;1;30;0
WireConnection;37;0;24;1
WireConnection;50;1;38;0
WireConnection;50;2;51;0
WireConnection;50;3;64;0
WireConnection;50;4;53;0
WireConnection;50;5;53;0
WireConnection;36;0;37;0
WireConnection;36;1;35;0
WireConnection;54;0;50;0
WireConnection;54;1;25;0
WireConnection;69;0;39;0
WireConnection;69;1;68;0
WireConnection;38;0;69;0
WireConnection;38;1;36;0
WireConnection;0;2;54;0
WireConnection;0;10;25;0
WireConnection;78;0;72;0
WireConnection;78;1;8;0
WireConnection;74;0;73;0
WireConnection;74;1;8;0
WireConnection;72;0;71;0
WireConnection;72;1;74;0
WireConnection;8;0;7;0
WireConnection;18;0;16;0
WireConnection;18;1;17;0
WireConnection;81;0;79;0
WireConnection;81;1;12;0
WireConnection;16;0;81;0
WireConnection;16;1;15;0
WireConnection;82;0;79;0
WireConnection;82;1;13;0
WireConnection;21;0;82;0
WireConnection;21;1;22;0
WireConnection;14;0;80;0
WireConnection;14;1;9;0
WireConnection;80;0;79;0
WireConnection;80;1;11;0
WireConnection;20;0;19;0
WireConnection;20;1;21;0
WireConnection;19;0;14;0
WireConnection;19;1;18;0
WireConnection;9;2;78;0
WireConnection;9;3;8;0
WireConnection;15;2;78;0
WireConnection;15;3;8;0
WireConnection;22;2;78;0
WireConnection;22;3;8;0
WireConnection;3;0;1;0
WireConnection;3;1;62;0
WireConnection;89;0;27;0
ASEEND*/
//CHKSM=4B52A3716E071ED36F384C7F6B08AF9522E4127A