// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "CY/1/Dissolve1"
{
	Properties
	{
		_Offset_Z("Offset_Z", Float) = 0
		_Depth("软边", Float) = 0
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode2("双面", Float) = 0
		[Toggle]_ZwrtteMode2("深度", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)]_AddOneBlendSrcAlpha("Add(One)Blend(Src Alpha)", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)]_AddOneBlendOneMinusSrcAlpha("Add(One)Blend(One Minus Src Alpha)", Float) = 10
		_Float23("颜色强度", Float) = 1
		_Float25("透明度", Float) = 1
		_Color2("主颜色", Color) = (1,1,1,1)
		_MainTex("主贴图", 2D) = "white" {}
		_Float16("主贴图U速度", Float) = 0
		_Float17("主贴图V速度", Float) = 0
		_Float18("主贴图旋转", Range( 0 , 360)) = 0
		[Toggle]_ToggleSwitch5("主贴图UV自定义", Float) = 1
		_Float20("主贴图扭曲强度", Float) = 0
		_MainTex1("副贴图", 2D) = "black" {}
		_Main1Desaturate1("去饱和度", Range( 0 , 1)) = 0
		_Float27("副贴图U速度", Float) = 0
		_Float26("副贴图V速度", Float) = 0
		_Float28("副贴图旋转", Range( 0 , 360)) = 0
		_Float29("副贴图扭曲强度", Float) = 0
		_DissolveTex("溶解贴图", 2D) = "white" {}
		_Float2("溶解U速度", Float) = 0
		_Float7("溶解V速度", Float) = 0
		_Float1("溶解旋转", Range( 0 , 360)) = 0
		_Float5("溶解参数", Range( 0 , 1)) = 0.4784614
		_Float9("软溶解", Range( 0.5 , 1)) = 0.5
		[Toggle]_ToggleSwitch4("溶解自定义", Float) = 0
		[Toggle]_ToggleSwitch6("溶解UV自定义", Float) = 0
		_Float19("溶解扭曲强度", Float) = 0
		[Toggle]_ToggleSwitch1("描边", Float) = 0
		_Color1("描边颜色", Color) = (1,1,1,1)
		_Float6("描边宽度", Range( 0 , 1)) = 0.15
		_Float10("描边强度", Float) = 1
		[Toggle]_ToggleSwitch0("方向溶解", Float) = 0
		_Float11("溶解方向", Range( 0 , 360)) = 0
		_Float15("方向U偏移", Float) = 0
		_Float12("方向V偏移", Float) = 0
		_Distortion1Tex1("扭曲贴图1", 2D) = "white" {}
		_Distortion1U("扭曲1U", Float) = 0
		_Distortion1V("扭曲1V", Float) = 0
		_Distortion1("扭曲1参数", Range( 0 , 1)) = 0
		_Distortion2Tex1("扭曲贴图2", 2D) = "white" {}
		Distortion2U("扭曲2U", Float) = 0
		_Distortion2V("扭曲2V", Float) = 0
		_Distortion2("扭曲2参数", Range( 0 , 1)) = 0
		_MaskTex2("遮罩贴图", 2D) = "white" {}
		_Mask1U1("遮罩U", Float) = 0
		_Mask1V1("遮罩V", Float) = 0
		_Mask1Rotation1("遮罩旋转", Range( 0 , 360)) = 0
		[Toggle]_ToggleSwitch3("遮罩UV自定义", Float) = 0
		_Float24("遮罩图扭曲强度", Float) = 0
		[HideInInspector] _tex4coord2( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _tex4coord3( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull [_CullMode2]
		ZWrite [_ZwrtteMode2]
		Blend [_AddOneBlendSrcAlpha] [_AddOneBlendOneMinusSrcAlpha]
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd vertex:vertexDataFunc 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float3 worldPos;
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
			float4 uv2_tex4coord2;
			float4 uv3_tex4coord3;
			float4 screenPos;
		};

		uniform float _Offset_Z;
		uniform half _ToggleSwitch1;
		uniform half _Float23;
		uniform half4 _Color2;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform half _Float18;
		uniform half _ToggleSwitch5;
		uniform half _Float16;
		uniform half _Float17;
		uniform sampler2D _Distortion1Tex1;
		uniform float4 _Distortion1Tex1_ST;
		uniform half _Distortion1U;
		uniform half _Distortion1V;
		uniform half _Distortion1;
		uniform sampler2D _Distortion2Tex1;
		uniform float4 _Distortion2Tex1_ST;
		uniform half Distortion2U;
		uniform half _Distortion2V;
		uniform half _Distortion2;
		uniform half _Float20;
		uniform sampler2D _MainTex1;
		uniform float4 _MainTex1_ST;
		uniform half _Float28;
		uniform half _Float27;
		uniform half _Float26;
		uniform half _Float29;
		uniform half _Main1Desaturate1;
		uniform half _Float10;
		uniform half4 _Color1;
		uniform half _Float9;
		uniform sampler2D _DissolveTex;
		uniform float4 _DissolveTex_ST;
		uniform half _Float1;
		uniform half _ToggleSwitch6;
		uniform half _Float2;
		uniform half _Float7;
		uniform half _Float19;
		uniform half _ToggleSwitch0;
		uniform half _Float15;
		uniform half _Float12;
		uniform half _Float11;
		uniform half _ToggleSwitch4;
		uniform half _Float5;
		uniform half _Float6;
		uniform half _Float25;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform half _Depth;
		uniform sampler2D _MaskTex2;
		uniform float4 _MaskTex2_ST;
		uniform half _Mask1Rotation1;
		uniform half _ToggleSwitch3;
		uniform half _Mask1U1;
		uniform half _Mask1V1;
		uniform half _Float24;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			half3 worldToObj724 = mul( unity_WorldToObject, float4( _WorldSpaceCameraPos, 1 ) ).xyz;
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			half3 worldToObj723 = mul( unity_WorldToObject, float4( ase_worldPos, 1 ) ).xyz;
			half3 normalizeResult727 = normalize( ( worldToObj724 - worldToObj723 ) );
			half3 Offset731 = ( normalizeResult727 * _Offset_Z );
			v.vertex.xyz += Offset731;
			v.vertex.w = 1;
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float cos6_g73 = cos( radians( _Float18 ) );
			float sin6_g73 = sin( radians( _Float18 ) );
			half2 rotator6_g73 = mul( uv_MainTex - half2( 0.5,0.5 ) , float2x2( cos6_g73 , -sin6_g73 , sin6_g73 , cos6_g73 )) + half2( 0.5,0.5 );
			half2 appendResult871 = (half2(_Float16 , _Float17));
			half2 appendResult875 = (half2(i.uv2_tex4coord2.x , i.uv2_tex4coord2.y));
			float2 uv_Distortion1Tex1 = i.uv_texcoord * _Distortion1Tex1_ST.xy + _Distortion1Tex1_ST.zw;
			half2 panner16_g11 = ( 1.0 * _Time.y * float2( 0,0 ) + uv_Distortion1Tex1);
			half2 appendResult8_g11 = (half2(_Distortion1U , _Distortion1V));
			half3 desaturateInitialColor758 = ( tex2D( _Distortion1Tex1, ( panner16_g11 + ( _Time.y * appendResult8_g11 ) ) ) * _Distortion1 ).rgb;
			half desaturateDot758 = dot( desaturateInitialColor758, float3( 0.299, 0.587, 0.114 ));
			half3 desaturateVar758 = lerp( desaturateInitialColor758, desaturateDot758.xxx, 1.0 );
			half3 Distortion1759 = desaturateVar758;
			float2 uv_Distortion2Tex1 = i.uv_texcoord * _Distortion2Tex1_ST.xy + _Distortion2Tex1_ST.zw;
			half2 panner16_g10 = ( 1.0 * _Time.y * float2( 0,0 ) + uv_Distortion2Tex1);
			half2 appendResult8_g10 = (half2(Distortion2U , _Distortion2V));
			half3 desaturateInitialColor757 = ( tex2D( _Distortion2Tex1, ( panner16_g10 + ( _Time.y * appendResult8_g10 ) ) ) * _Distortion2 ).rgb;
			half desaturateDot757 = dot( desaturateInitialColor757, float3( 0.299, 0.587, 0.114 ));
			half3 desaturateVar757 = lerp( desaturateInitialColor757, desaturateDot757.xxx, 1.0 );
			half3 Distortion2760 = desaturateVar757;
			half3 lerpResult766 = lerp( half3( ( rotator6_g73 + (( _ToggleSwitch5 )?( appendResult875 ):( ( _Time.y * appendResult871 ) )) ) ,  0.0 ) , ( Distortion1759 + Distortion2760 ) , _Float20);
			half4 tex2DNode89 = tex2D( _MainTex, lerpResult766.xy );
			half4 Main703 = tex2DNode89;
			float2 uv_MainTex1 = i.uv_texcoord * _MainTex1_ST.xy + _MainTex1_ST.zw;
			float cos6_g75 = cos( radians( _Float28 ) );
			float sin6_g75 = sin( radians( _Float28 ) );
			half2 rotator6_g75 = mul( uv_MainTex1 - half2( 0.5,0.5 ) , float2x2( cos6_g75 , -sin6_g75 , sin6_g75 , cos6_g75 )) + half2( 0.5,0.5 );
			half2 appendResult904 = (half2(_Float27 , _Float26));
			half3 lerpResult917 = lerp( half3( ( rotator6_g75 + ( _Time.y * appendResult904 ) ) ,  0.0 ) , ( Distortion1759 + Distortion2760 ) , _Float29);
			half3 desaturateInitialColor927 = (tex2D( _MainTex1, lerpResult917.xy )).rgb;
			half desaturateDot927 = dot( desaturateInitialColor927, float3( 0.299, 0.587, 0.114 ));
			half3 desaturateVar927 = lerp( desaturateInitialColor927, desaturateDot927.xxx, _Main1Desaturate1 );
			half3 Main1919 = desaturateVar927;
			half4 temp_output_771_0 = ( _Float23 * ( ( _Color2 * i.vertexColor * Main703 ) + half4( Main1919 , 0.0 ) ) );
			half temp_output_577_0 = ( 1.0 - _Float9 );
			float2 uv_DissolveTex = i.uv_texcoord * _DissolveTex_ST.xy + _DissolveTex_ST.zw;
			float cos6_g71 = cos( radians( _Float1 ) );
			float sin6_g71 = sin( radians( _Float1 ) );
			half2 rotator6_g71 = mul( uv_DissolveTex - half2( 0.5,0.5 ) , float2x2( cos6_g71 , -sin6_g71 , sin6_g71 , cos6_g71 )) + half2( 0.5,0.5 );
			half2 appendResult880 = (half2(_Float2 , _Float7));
			half2 appendResult883 = (half2(i.uv3_tex4coord3.y , i.uv3_tex4coord3.z));
			half3 lerpResult761 = lerp( half3( ( rotator6_g71 + (( _ToggleSwitch6 )?( appendResult883 ):( ( _Time.y * appendResult880 ) )) ) ,  0.0 ) , ( Distortion1759 + Distortion2760 ) , _Float19);
			half2 appendResult686 = (half2(_Float15 , _Float12));
			float2 uv_TexCoord680 = i.uv_texcoord + appendResult686;
			float cos683 = cos( radians( _Float11 ) );
			float sin683 = sin( radians( _Float11 ) );
			half2 rotator683 = mul( uv_TexCoord680 - half2( 0.5,0.5 ) , float2x2( cos683 , -sin683 , sin683 , cos683 )) + half2( 0.5,0.5 );
			half temp_output_663_0 = ( tex2D( _DissolveTex, lerpResult761.xy ).r * (( _ToggleSwitch0 )?( (rotator683).x ):( 1.0 )) );
			half smoothstepResult564 = smoothstep( temp_output_577_0 , _Float9 , saturate( ( temp_output_663_0 + 1.0 + ( -2.0 * (( _ToggleSwitch4 )?( i.uv3_tex4coord3.x ):( _Float5 )) ) ) ));
			half smoothstepResult626 = smoothstep( temp_output_577_0 , _Float9 , saturate( ( temp_output_663_0 + 1.0 + ( ( (( _ToggleSwitch4 )?( i.uv3_tex4coord3.x ):( _Float5 )) * ( 1.0 + _Float6 ) ) * -2.0 ) ) ));
			half DissolveLine605 = ( smoothstepResult564 - smoothstepResult626 );
			half4 lerpResult634 = lerp( temp_output_771_0 , ( _Float10 * _Color1 * DissolveLine605 ) , DissolveLine605);
			o.Emission = (( _ToggleSwitch1 )?( lerpResult634 ):( temp_output_771_0 )).rgb;
			half Alpha706 = tex2DNode89.a;
			half Dissolve591 = smoothstepResult564;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			half4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth722 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			half distanceDepth722 = abs( ( screenDepth722 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Depth ) );
			half clampResult725 = clamp( distanceDepth722 , 0.0 , 1.0 );
			half Depth728 = clampResult725;
			float2 uv_MaskTex2 = i.uv_texcoord * _MaskTex2_ST.xy + _MaskTex2_ST.zw;
			float cos6_g76 = cos( radians( _Mask1Rotation1 ) );
			float sin6_g76 = sin( radians( _Mask1Rotation1 ) );
			half2 rotator6_g76 = mul( uv_MaskTex2 - half2( 0.5,0.5 ) , float2x2( cos6_g76 , -sin6_g76 , sin6_g76 , cos6_g76 )) + half2( 0.5,0.5 );
			half2 appendResult861 = (half2(_Mask1U1 , _Mask1V1));
			half2 appendResult868 = (half2(i.uv2_tex4coord2.z , i.uv2_tex4coord2.w));
			half3 lerpResult898 = lerp( half3( ( rotator6_g76 + (( _ToggleSwitch3 )?( appendResult868 ):( ( _Time.y * appendResult861 ) )) ) ,  0.0 ) , ( Distortion1759 + Distortion2760 ) , _Float24);
			half Mask1806 = tex2D( _MaskTex2, lerpResult898.xy ).r;
			o.Alpha = ( _Float25 * ( ( _Color2.a * i.vertexColor.a * Alpha706 ) * Dissolve591 * Depth728 * Mask1806 ) );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18900
2553;48;2560;1331;4580.602;-963.0232;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;735;-7380.919,66.45311;Inherit;False;1661.234;460.8827;UV扭曲1;9;759;758;756;752;751;748;739;740;786;UV扭曲1;0.8514075,0.5707102,0.8962264,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;736;-7373.409,668.8082;Inherit;False;1627.432;453.1665;UV扭曲2;9;760;757;755;753;754;745;737;738;785;UV扭曲2;0.8514075,0.5707102,0.8962264,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;738;-7248.758,895.0507;Half;False;Property;Distortion2U;扭曲2U;43;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;737;-7260.829,1002.566;Half;False;Property;_Distortion2V;扭曲2V;44;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;740;-7277.889,292.9712;Half;False;Property;_Distortion1U;扭曲1U;39;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;739;-7286.027,412.2793;Half;False;Property;_Distortion1V;扭曲1V;40;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;748;-7340.919,117.4531;Inherit;False;0;751;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;745;-7290.787,734.6099;Inherit;False;0;753;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;785;-7021.289,822.2693;Inherit;False;UV1;-1;;10;19c16b1c24df1c1449ec7fe728b6b99f;0;3;13;FLOAT2;0,0;False;14;FLOAT;0;False;15;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;786;-7044.628,241.2252;Inherit;False;UV1;-1;;11;19c16b1c24df1c1449ec7fe728b6b99f;0;3;13;FLOAT2;0,0;False;14;FLOAT;0;False;15;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;752;-6692.919,434.4524;Inherit;False;Property;_Distortion1;扭曲1参数;41;0;Create;False;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;753;-6717.408,796.8084;Inherit;True;Property;_Distortion2Tex1;扭曲贴图2;42;0;Create;False;0;0;0;False;0;False;-1;None;87cf9d80462c4114481e3faed0499f7e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;754;-6733.408,1004.808;Inherit;False;Property;_Distortion2;扭曲2参数;45;0;Create;False;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;751;-6724.919,210.4531;Inherit;True;Property;_Distortion1Tex1;扭曲贴图1;38;0;Create;False;0;0;0;False;0;False;-1;None;bce71000569f9e442a68fd0afad678d3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;590;-7861.636,2481.137;Inherit;False;3980.002;752.306;溶解;34;814;883;884;676;677;882;880;879;591;696;564;563;645;644;577;557;693;561;576;558;560;813;812;697;562;534;761;765;764;763;672;762;675;891;溶解;0.5330188,0.9809342,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;676;-7832.749,2852.029;Inherit;False;Property;_Float2;溶解U速度;22;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;677;-7833.628,2940.493;Inherit;False;Property;_Float7;溶解V速度;23;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;756;-6388.919,322.4521;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;755;-6413.409,892.8085;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DesaturateOpNode;757;-6253.409,796.8084;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;880;-7619.647,2892.463;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;814;-7664.069,3009.293;Inherit;False;2;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;879;-7638.595,2805.521;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DesaturateOpNode;758;-6212.919,210.4531;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;694;-7475.62,3469.386;Inherit;False;1847.433;474.8018;溶解方向;12;683;680;685;686;688;687;681;682;684;692;691;663;溶解方向;0.5518868,0.6059949,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;760;-5979.593,793.1293;Inherit;False;Distortion2;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;759;-5959.792,203.9506;Inherit;False;Distortion1;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;883;-7413.329,3057.699;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;688;-7425.62,3534.606;Inherit;False;Property;_Float15;方向U偏移;36;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;882;-7390.957,2839.283;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;687;-7424.467,3624.97;Inherit;False;Property;_Float12;方向V偏移;37;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;763;-6948.577,2893.155;Inherit;False;760;Distortion2;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;686;-7210.543,3576.289;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;684;-7240.043,3828.188;Inherit;False;Property;_Float11;溶解方向;35;0;Create;False;0;0;0;False;0;False;0;65;0;360;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;884;-7224.787,2835.754;Inherit;False;Property;_ToggleSwitch6;溶解UV自定义;28;0;Create;False;0;0;0;False;0;False;0;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;762;-6946.378,2806.554;Inherit;False;759;Distortion1;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;675;-7594.006,2706.323;Inherit;False;Property;_Float1;溶解旋转;24;0;Create;False;0;0;0;False;0;False;0;141;0;360;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;672;-7555.704,2569.339;Inherit;False;0;534;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RadiansOpNode;682;-6889.705,3812.776;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;681;-6964.899,3675.481;Inherit;False;Constant;_Vector0;Vector 0;20;0;Create;True;0;0;0;False;0;False;0.5,0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.FunctionNode;891;-6927.467,2637.474;Inherit;False;UV2;-1;;71;c598558fc7ea77e4f949a712cde78db3;0;3;13;FLOAT2;0,0;False;12;FLOAT;0;False;14;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;764;-6778.633,3021.293;Inherit;False;Property;_Float19;溶解扭曲强度;29;0;Create;False;0;0;0;False;0;False;0;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;765;-6724.33,2820.49;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;680;-7043.895,3531.245;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;901;-5541.879,1411.382;Inherit;False;2393.551;879.3185;副贴图;16;924;919;918;917;916;914;915;908;909;910;911;912;904;905;902;903;副贴图;0.9811321,0.7617207,0.4026344,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;704;-5527.471,392.1201;Inherit;False;2040.551;891.3185;主贴图;19;873;698;699;706;703;89;766;770;767;700;701;876;769;768;874;875;872;871;893;主贴图;0.9811321,0.7617207,0.4026344,1;0;0
Node;AmplifyShaderEditor.LerpOp;761;-6518.552,2708.563;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RotatorNode;683;-6650.718,3652.771;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;902;-5452.347,1899.903;Inherit;False;Property;_Float26;副贴图V速度;18;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;903;-5446.422,1797.344;Inherit;False;Property;_Float27;副贴图U速度;17;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;692;-6343.46,3519.386;Inherit;False;Constant;_Float0;Float 0;16;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;699;-5494.412,877.6158;Inherit;False;Property;_Float17;主贴图V速度;11;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;685;-6409.545,3649.388;Inherit;False;True;False;True;True;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;534;-6292.017,2729.647;Inherit;True;Property;_DissolveTex;溶解贴图;21;0;Create;False;0;0;0;False;0;False;-1;5d34447d6cf644949bef961af1bc9e79;10105a88788cf2347afffcb3ba8524bb;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;646;-5784.048,4106.082;Inherit;False;2298.67;380.9553;描边;12;605;633;626;631;630;627;629;628;595;597;598;596;溶解描边;1,0.2783019,0.2783019,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;698;-5497.566,798.2516;Inherit;False;Property;_Float16;主贴图U速度;10;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;904;-5226.668,1852.909;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;872;-5297.768,757.7965;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;596;-5744.226,4328.666;Inherit;False;Property;_Float6;描边宽度;32;0;Create;False;0;0;0;False;0;False;0.15;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;905;-5245.617,1765.965;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;598;-5648.226,4248.665;Inherit;False;Constant;_Float8;Float 8;11;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;691;-6139.358,3612.988;Inherit;False;Property;_ToggleSwitch0;方向溶解;34;0;Create;False;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;697;-5959.62,2954.238;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;562;-5936.208,2890.915;Inherit;False;Property;_Float5;溶解参数;25;0;Create;False;0;0;0;False;0;False;0.4784614;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;871;-5278.82,844.7403;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;873;-5332.759,984.3099;Inherit;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;812;-5900.053,2981.346;Inherit;False;2;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;912;-4890.334,2008.364;Inherit;False;760;Distortion2;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;911;-4888.135,1921.765;Inherit;False;759;Distortion1;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;910;-5319.612,1655.16;Inherit;False;Property;_Float28;副贴图旋转;19;0;Create;False;0;0;0;False;0;False;0;0;0;360;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;909;-5287.752,1500.336;Inherit;False;0;918;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;908;-4997.979,1799.729;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;792;-2892.549,2482.77;Inherit;False;2107.044;735.9767;;15;796;795;804;806;794;793;809;861;862;863;867;868;892;898;896;Mask1;0.2653524,0.7495487,0.8396226,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;597;-5424.226,4264.665;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;874;-5050.13,791.5604;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;875;-5067.108,1001.321;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;560;-5581.532,2727.925;Inherit;False;Constant;_Float4;Float 4;7;0;Create;True;0;0;0;False;0;False;-2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;813;-5562.763,3014.13;Inherit;False;Property;_ToggleSwitch4;溶解自定义;27;0;Create;False;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;663;-5863.188,3561.197;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;914;-4653.565,1944.423;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;916;-4714.691,2089.731;Inherit;False;Property;_Float29;副贴图扭曲强度;20;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;915;-4795.68,1633.806;Inherit;False;UV2;-1;;75;c598558fc7ea77e4f949a712cde78db3;0;3;13;FLOAT2;0,0;False;12;FLOAT;0;False;14;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;768;-4616.077,800.0698;Inherit;False;759;Distortion1;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;561;-5274.554,2735.294;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;693;-5670.663,2671.059;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;628;-5248.226,4392.666;Inherit;False;Constant;_Float13;Float 13;7;0;Create;True;0;0;0;False;0;False;-2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;701;-5188.274,457.8819;Inherit;False;0;89;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;793;-2853.882,2951.125;Half;False;Property;_Mask1V1;遮罩V;48;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;576;-5313.53,2884.374;Inherit;False;Property;_Float9;软溶解;26;0;Create;False;0;0;0;False;0;False;0.5;0.515;0.5;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;595;-5232.226,4248.665;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;876;-4883.96,788.0313;Inherit;False;Property;_ToggleSwitch5;主贴图UV自定义;13;0;Create;False;0;0;0;False;0;False;1;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;794;-2854.113,2843.989;Half;False;Property;_Mask1U1;遮罩U;47;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;558;-5283.682,2619.28;Inherit;False;Constant;_Float3;Float 3;7;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;769;-4618.277,886.6688;Inherit;False;760;Distortion2;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;700;-5221.142,600.6036;Inherit;False;Property;_Float18;主贴图旋转;12;0;Create;False;0;0;0;False;0;False;0;0;0;360;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;917;-4432.095,1914.696;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;695;-5361.464,3682.974;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;629;-5040.226,4248.665;Inherit;False;Constant;_Float14;Float 14;7;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;861;-2651.414,2867.348;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;577;-4925.098,2870.885;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;627;-5040.226,4344.666;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;893;-4638.72,557.0633;Inherit;False;UV2;-1;;73;c598558fc7ea77e4f949a712cde78db3;0;3;13;FLOAT2;0,0;False;12;FLOAT;0;False;14;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;557;-5017.339,2590.655;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;809;-2723.414,3015.544;Inherit;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;770;-4441.625,968.0355;Inherit;False;Property;_Float20;主贴图扭曲强度;14;0;Create;False;0;0;0;False;0;False;0;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;767;-4381.508,822.7277;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleTimeNode;863;-2670.361,2780.405;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;630;-4803.729,4260.781;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;862;-2422.723,2814.168;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;766;-4193.317,840.4006;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;563;-4819.242,2585.917;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;645;-4730.311,2811.932;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;868;-2433.963,3026.798;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;924;-3894.883,1617.762;Inherit;False;466;301.4021;去饱和度;3;927;926;925;去饱和度;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;918;-4225.164,1682.113;Inherit;True;Property;_MainTex1;副贴图;15;0;Create;False;0;0;0;False;0;False;-1;44837dcfd4573cb45949d77546e74a4e;7cdd2c3718c052444b448adaa6805757;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;644;-4940.278,2800.812;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;867;-2246.511,2843.631;Inherit;False;Property;_ToggleSwitch3;遮罩UV自定义;50;0;Create;False;0;0;0;False;0;False;0;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;926;-3852.558,1679.624;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SmoothstepOpNode;564;-4578.253,2703.018;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;642;-4717.399,3472.928;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;796;-2673.31,2530.846;Inherit;False;0;804;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;894;-2201.242,3083.83;Inherit;False;760;Distortion2;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;795;-2671.063,2669.308;Inherit;False;Property;_Mask1Rotation1;遮罩旋转;49;0;Create;False;0;0;0;False;0;False;0;90;0;360;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;718;198.6832,2656.133;Inherit;False;1422.961;459.0378;;9;731;730;729;727;726;724;723;721;719;Offset;0.8679245,0.7715256,0.2824849,1;0;0
Node;AmplifyShaderEditor.WireNode;643;-4954.895,3719.129;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;89;-4052.653,510.837;Inherit;True;Property;_MainTex;主贴图;9;0;Create;False;0;0;0;False;0;False;-1;44837dcfd4573cb45949d77546e74a4e;39b7c50316e13b747be068cdf2bde292;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;631;-4632.999,4267.995;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;717;209.3493,2250.998;Inherit;False;1031.434;231.4038;;4;728;725;722;720;Depth;0.4053043,0.4369667,0.8679245,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;925;-3872.084,1799.948;Inherit;False;Property;_Main1Desaturate1;去饱和度;16;0;Create;False;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;895;-2199.042,2997.231;Inherit;False;759;Distortion1;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;703;-3733.209,504.8215;Inherit;False;Main;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;696;-4292.461,3115.506;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DesaturateOpNode;927;-3596.883,1683.758;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;897;-2024.59,3165.197;Inherit;False;Property;_Float24;遮罩图扭曲强度;51;0;Create;False;0;0;0;False;0;False;0;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;896;-1968.473,2985.889;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldSpaceCameraPos;721;256.802,2739.146;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SmoothstepOpNode;626;-4414.457,4203.811;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;720;276.396,2309.375;Inherit;False;Property;_Depth;软边;1;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;892;-2108.948,2584.805;Inherit;False;UV2;-1;;76;c598558fc7ea77e4f949a712cde78db3;0;3;13;FLOAT2;0,0;False;12;FLOAT;0;False;14;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WorldPosInputsNode;719;337.1301,2914.305;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.VertexColorNode;651;-3233.571,707.5878;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TransformPositionNode;724;562.799,2733.793;Inherit;False;World;Object;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;633;-4042.462,4188.042;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;722;490.2841,2300.999;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;919;-3362.617,1634.075;Inherit;False;Main1;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;612;-3263.252,524.8469;Inherit;False;Property;_Color2;主颜色;8;0;Create;False;0;0;0;False;0;False;1,1,1,1;0.06792016,0.08329837,0.305,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TransformPositionNode;723;558.1291,2909.305;Inherit;False;World;Object;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;705;-3225.108,897.2303;Inherit;False;703;Main;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;898;-1785.282,2887.562;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;706;-3729.073,615.4294;Inherit;False;Alpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;726;839.1302,2830.305;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;804;-1642.614,2594.624;Inherit;True;Property;_MaskTex2;遮罩贴图;46;0;Create;False;0;0;0;False;0;False;-1;None;6fe6615117650be41a4950d240ac2d5f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;649;-2825.312,605.198;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;921;-3224.622,1014.461;Inherit;False;919;Main1;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;605;-3726.073,4201.823;Inherit;False;DissolveLine;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;647;-2938.167,-48.97146;Inherit;False;1217.005;464.6765;描边;6;634;604;609;602;603;773;描边;1,0.5518868,0.5956227,1;0;0
Node;AmplifyShaderEditor.ClampOpNode;725;754.3481,2302.897;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;603;-2888.167,88.03578;Inherit;False;Property;_Color1;描边颜色;31;0;Create;False;0;0;0;False;0;False;1,1,1,1;0,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;772;-2757.089,462.7007;Inherit;False;Property;_Float23;颜色强度;6;0;Create;False;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;728;1001.14,2339.663;Inherit;False;Depth;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;609;-2869.409,268.8425;Inherit;False;605;DissolveLine;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;923;-2641.936,668.6;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalizeNode;727;1011.293,2843.662;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;707;-3219.017,1157.289;Inherit;False;706;Alpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;806;-1204.281,2613.363;Inherit;False;Mask1;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;729;957.2933,2924.662;Float;False;Property;_Offset_Z;Offset_Z;0;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;591;-4212.694,2630.376;Inherit;False;Dissolve;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;602;-2861.242,1.028556;Inherit;False;Property;_Float10;描边强度;33;0;Create;False;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;771;-2479.728,537.7015;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;604;-2570.009,50.36698;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;811;-2581.058,1461.411;Inherit;False;806;Mask1;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;733;-2595.28,1352.86;Inherit;False;728;Depth;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;652;-2710.279,1005.218;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;608;-2602.896,1241.992;Inherit;False;591;Dissolve;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;709;-5322.63,-264.1891;Inherit;False;780.6375;517.3364;一般参数;3;712;711;710;一般参数;0.0518868,0.9175034,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;730;1178.092,2907.662;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;731;1397.608,2902.947;Inherit;False;Offset;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;547;-2227.715,1186.241;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;634;-2279.492,160.2286;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;712;-4920.993,-214.1891;Inherit;False;329;160;深度;1;715;深度;1,0.1933962,0.1933962,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;899;-2206.795,761.4202;Inherit;False;Property;_Float25;透明度;7;0;Create;False;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;711;-5272.63,-211.3741;Inherit;False;329;160;双面;1;713;双面;1,0.1933962,0.1933962,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;710;-5269.549,-0.8517313;Inherit;False;334;251;混和模式;2;716;714;混和模式;1,0.1933962,0.1933962,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;715;-4858.787,-162.2548;Inherit;False;Property;_ZwrtteMode2;深度;3;1;[Toggle];Fetch;False;0;0;0;True;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;714;-5230.549,50.14819;Inherit;False;Property;_AddOneBlendSrcAlpha;Add(One)Blend(Src Alpha);4;1;[Enum];Fetch;True;0;0;1;UnityEngine.Rendering.BlendMode;True;0;False;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;900;-1998.22,862.1926;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;716;-5229.549,137.1482;Inherit;False;Property;_AddOneBlendOneMinusSrcAlpha;Add(One)Blend(One Minus Src Alpha);5;1;[Enum];Fetch;True;0;0;1;UnityEngine.Rendering.BlendMode;True;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;734;-1921.422,1299.048;Inherit;False;731;Offset;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;713;-5210.425,-159.4399;Inherit;False;Property;_CullMode2;双面;2;1;[Enum];Fetch;False;0;0;1;UnityEngine.Rendering.CullMode;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;773;-1993.369,231.4056;Inherit;False;Property;_ToggleSwitch1;描边;30;0;Create;False;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-1439.166,554.564;Half;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;CY/1/Dissolve1;False;False;False;False;True;True;True;True;True;True;True;True;False;False;False;False;False;False;False;False;False;Off;0;True;715;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Custom;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;True;714;10;True;716;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;52;-1;-1;-1;0;False;0;0;True;713;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;785;13;745;0
WireConnection;785;14;738;0
WireConnection;785;15;737;0
WireConnection;786;13;748;0
WireConnection;786;14;740;0
WireConnection;786;15;739;0
WireConnection;753;1;785;0
WireConnection;751;1;786;0
WireConnection;756;0;751;0
WireConnection;756;1;752;0
WireConnection;755;0;753;0
WireConnection;755;1;754;0
WireConnection;757;0;755;0
WireConnection;880;0;676;0
WireConnection;880;1;677;0
WireConnection;758;0;756;0
WireConnection;760;0;757;0
WireConnection;759;0;758;0
WireConnection;883;0;814;2
WireConnection;883;1;814;3
WireConnection;882;0;879;0
WireConnection;882;1;880;0
WireConnection;686;0;688;0
WireConnection;686;1;687;0
WireConnection;884;0;882;0
WireConnection;884;1;883;0
WireConnection;682;0;684;0
WireConnection;891;13;672;0
WireConnection;891;12;675;0
WireConnection;891;14;884;0
WireConnection;765;0;762;0
WireConnection;765;1;763;0
WireConnection;680;1;686;0
WireConnection;761;0;891;0
WireConnection;761;1;765;0
WireConnection;761;2;764;0
WireConnection;683;0;680;0
WireConnection;683;1;681;0
WireConnection;683;2;682;0
WireConnection;685;0;683;0
WireConnection;534;1;761;0
WireConnection;904;0;903;0
WireConnection;904;1;902;0
WireConnection;691;0;692;0
WireConnection;691;1;685;0
WireConnection;697;0;534;1
WireConnection;871;0;698;0
WireConnection;871;1;699;0
WireConnection;908;0;905;0
WireConnection;908;1;904;0
WireConnection;597;0;598;0
WireConnection;597;1;596;0
WireConnection;874;0;872;0
WireConnection;874;1;871;0
WireConnection;875;0;873;1
WireConnection;875;1;873;2
WireConnection;813;0;562;0
WireConnection;813;1;812;1
WireConnection;663;0;697;0
WireConnection;663;1;691;0
WireConnection;914;0;911;0
WireConnection;914;1;912;0
WireConnection;915;13;909;0
WireConnection;915;12;910;0
WireConnection;915;14;908;0
WireConnection;561;0;560;0
WireConnection;561;1;813;0
WireConnection;693;0;663;0
WireConnection;595;0;813;0
WireConnection;595;1;597;0
WireConnection;876;0;874;0
WireConnection;876;1;875;0
WireConnection;917;0;915;0
WireConnection;917;1;914;0
WireConnection;917;2;916;0
WireConnection;695;0;663;0
WireConnection;861;0;794;0
WireConnection;861;1;793;0
WireConnection;577;0;576;0
WireConnection;627;0;595;0
WireConnection;627;1;628;0
WireConnection;893;13;701;0
WireConnection;893;12;700;0
WireConnection;893;14;876;0
WireConnection;557;0;693;0
WireConnection;557;1;558;0
WireConnection;557;2;561;0
WireConnection;767;0;768;0
WireConnection;767;1;769;0
WireConnection;630;0;695;0
WireConnection;630;1;629;0
WireConnection;630;2;627;0
WireConnection;862;0;863;0
WireConnection;862;1;861;0
WireConnection;766;0;893;0
WireConnection;766;1;767;0
WireConnection;766;2;770;0
WireConnection;563;0;557;0
WireConnection;645;0;577;0
WireConnection;868;0;809;3
WireConnection;868;1;809;4
WireConnection;918;1;917;0
WireConnection;644;0;576;0
WireConnection;867;0;862;0
WireConnection;867;1;868;0
WireConnection;926;0;918;0
WireConnection;564;0;563;0
WireConnection;564;1;645;0
WireConnection;564;2;644;0
WireConnection;642;0;577;0
WireConnection;643;0;576;0
WireConnection;89;1;766;0
WireConnection;631;0;630;0
WireConnection;703;0;89;0
WireConnection;696;0;564;0
WireConnection;927;0;926;0
WireConnection;927;1;925;0
WireConnection;896;0;895;0
WireConnection;896;1;894;0
WireConnection;626;0;631;0
WireConnection;626;1;642;0
WireConnection;626;2;643;0
WireConnection;892;13;796;0
WireConnection;892;12;795;0
WireConnection;892;14;867;0
WireConnection;724;0;721;0
WireConnection;633;0;696;0
WireConnection;633;1;626;0
WireConnection;722;0;720;0
WireConnection;919;0;927;0
WireConnection;723;0;719;0
WireConnection;898;0;892;0
WireConnection;898;1;896;0
WireConnection;898;2;897;0
WireConnection;706;0;89;4
WireConnection;726;0;724;0
WireConnection;726;1;723;0
WireConnection;804;1;898;0
WireConnection;649;0;612;0
WireConnection;649;1;651;0
WireConnection;649;2;705;0
WireConnection;605;0;633;0
WireConnection;725;0;722;0
WireConnection;728;0;725;0
WireConnection;923;0;649;0
WireConnection;923;1;921;0
WireConnection;727;0;726;0
WireConnection;806;0;804;1
WireConnection;591;0;564;0
WireConnection;771;0;772;0
WireConnection;771;1;923;0
WireConnection;604;0;602;0
WireConnection;604;1;603;0
WireConnection;604;2;609;0
WireConnection;652;0;612;4
WireConnection;652;1;651;4
WireConnection;652;2;707;0
WireConnection;730;0;727;0
WireConnection;730;1;729;0
WireConnection;731;0;730;0
WireConnection;547;0;652;0
WireConnection;547;1;608;0
WireConnection;547;2;733;0
WireConnection;547;3;811;0
WireConnection;634;0;771;0
WireConnection;634;1;604;0
WireConnection;634;2;609;0
WireConnection;900;0;899;0
WireConnection;900;1;547;0
WireConnection;773;0;771;0
WireConnection;773;1;634;0
WireConnection;0;2;773;0
WireConnection;0;9;900;0
WireConnection;0;11;734;0
ASEEND*/
//CHKSM=FB8110EC7B7872916BBBCAA81CB41828FBC836DF