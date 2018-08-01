using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using System.Text;
using System.Collections.Generic;

namespace OpenGL
{
	public static class GL
    {
        private static readonly uint[] singleHandle = new uint[1];

        /// <summary>
        /// Set to true if we're inside glBegin.
        /// </summary>
        private static bool insideGLBegin = false;

        #region The OpenGL constant definitions.

        //   OpenGL Version Identifier
        public const uint GL_VERSION_1_1 = 1;

	    //  AccumOp
		public const uint GL_ACCUM                          = 0x0100;
		public const uint GL_LOAD                           = 0x0101;
		public const uint GL_RETURN                         = 0x0102;
		public const uint GL_MULT                           = 0x0103;
		public const uint GL_ADD                            = 0x0104;

        //  Alpha functions
		public const uint GL_NEVER                          = 0x0200;
		public const uint GL_LESS                           = 0x0201;
		public const uint GL_EQUAL                          = 0x0202;
		public const uint GL_LEQUAL                         = 0x0203;
		public const uint GL_GREATER                        = 0x0204;
		public const uint GL_NOTEQUAL                       = 0x0205;
		public const uint GL_GEQUAL                         = 0x0206;
		public const uint GL_ALWAYS                         = 0x0207;

	    //  AttribMask
		public const uint GL_CURRENT_BIT                    = 0x00000001;
		public const uint GL_POINT_BIT                      = 0x00000002;
		public const uint GL_LINE_BIT                       = 0x00000004;
		public const uint GL_POLYGON_BIT                    = 0x00000008;
		public const uint GL_POLYGON_STIPPLE_BIT            = 0x00000010;
		public const uint GL_PIXEL_MODE_BIT                 = 0x00000020;
		public const uint GL_LIGHTING_BIT                   = 0x00000040;
		public const uint GL_FOG_BIT                        = 0x00000080;
		public const uint GL_DEPTH_BUFFER_BIT               = 0x00000100;
		public const uint GL_ACCUM_BUFFER_BIT               = 0x00000200;
		public const uint GL_STENCIL_BUFFER_BIT             = 0x00000400;
		public const uint GL_VIEWPORT_BIT                   = 0x00000800;
		public const uint GL_TRANSFORM_BIT                  = 0x00001000;
		public const uint GL_ENABLE_BIT                     = 0x00002000;
		public const uint GL_COLOR_BUFFER_BIT               = 0x00004000;
		public const uint GL_HINT_BIT                       = 0x00008000;
		public const uint GL_EVAL_BIT                       = 0x00010000;
		public const uint GL_LIST_BIT                       = 0x00020000;
		public const uint GL_TEXTURE_BIT                    = 0x00040000;
		public const uint GL_SCISSOR_BIT                    = 0x00080000;
		public const uint GL_ALL_ATTRIB_BITS                = 0x000fffff;

	    //  BeginMode

        /// <summary>
        /// Treats each vertex as a single point. Vertex n defines point n. N points are drawn.
        /// </summary>
		public const uint GL_POINTS                         = 0x0000;

        /// <summary>
        /// Treats each pair of vertices as an independent line segment. Vertices 2n - 1 and 2n define line n. N/2 lines are drawn.
        /// </summary>
		public const uint GL_LINES                          = 0x0001;

        /// <summary>
        /// Draws a connected group of line segments from the first vertex to the last, then back to the first. Vertices n and n + 1 define line n. The last line, however, is defined by vertices N and 1. N lines are drawn.
        /// </summary>
		public const uint GL_LINE_LOOP                      = 0x0002;

        /// <summary>
        /// Draws a connected group of line segments from the first vertex to the last. Vertices n and n+1 define line n. N - 1 lines are drawn.
        /// </summary>
		public const uint GL_LINE_STRIP                     = 0x0003;

        /// <summary>
        /// Treats each triplet of vertices as an independent triangle. Vertices 3n - 2, 3n - 1, and 3n define triangle n. N/3 triangles are drawn.
        /// </summary>
		public const uint GL_TRIANGLES                      = 0x0004;

        /// <summary>
        /// Draws a connected group of triangles. One triangle is defined for each vertex presented after the first two vertices. For odd n, vertices n, n + 1, and n + 2 define triangle n. For even n, vertices n + 1, n, and n + 2 define triangle n. N - 2 triangles are drawn.
        /// </summary>
		public const uint GL_TRIANGLE_STRIP                 = 0x0005;

	    /// <summary>
	    /// Draws a connected group of triangles. one triangle is defined for each vertex presented after the first two vertices. Vertices 1, n + 1, n + 2 define triangle n. N - 2 triangles are drawn.
	    /// </summary>
	    public const uint GL_TRIANGLE_FAN = 0x0006;

        /// <summary>
        /// Treats each group of four vertices as an independent quadrilateral. Vertices 4n - 3, 4n - 2, 4n - 1, and 4n define quadrilateral n. N/4 quadrilaterals are drawn.
        /// </summary>
		public const uint GL_QUADS                          = 0x0007;

        /// <summary>
        /// Draws a connected group of quadrilaterals. One quadrilateral is defined for each pair of vertices presented after the first pair. Vertices 2n - 1, 2n, 2n + 2, and 2n + 1 define quadrilateral n. N/2 - 1 quadrilaterals are drawn. Note that the order in which vertices are used to construct a quadrilateral from strip data is different from that used with independent data.
        /// </summary>
		public const uint GL_QUAD_STRIP                     = 0x0008;

        /// <summary>
        /// Draws a single, convex polygon. Vertices 1 through N define this polygon.
        /// </summary>
        public const uint GL_POLYGON                        = 0x0009;

	    //  BlendingFactorDest
		public const uint GL_ZERO                           = 0;
		public const uint GL_ONE                            = 1;
		public const uint GL_SRC_COLOR                      = 0x0300;
		public const uint GL_ONE_MINUS_SRC_COLOR            = 0x0301;
		public const uint GL_SRC_ALPHA                      = 0x0302;
		public const uint GL_ONE_MINUS_SRC_ALPHA            = 0x0303;
		public const uint GL_DST_ALPHA                      = 0x0304;
		public const uint GL_ONE_MINUS_DST_ALPHA            = 0x0305;

	    //  BlendingFactorSrc
		public const uint GL_DST_COLOR                      = 0x0306;
		public const uint GL_ONE_MINUS_DST_COLOR            = 0x0307;
		public const uint GL_SRC_ALPHA_SATURATE             = 0x0308;
		
	    //   Boolean
		public const uint GL_TRUE                           = 1;
		public const uint GL_FALSE                          = 0;
		     
	    //   ClipPlaneName
		public const uint GL_CLIP_PLANE0                    = 0x3000;
		public const uint GL_CLIP_PLANE1                    = 0x3001;
		public const uint GL_CLIP_PLANE2                    = 0x3002;
		public const uint GL_CLIP_PLANE3                    = 0x3003;
		public const uint GL_CLIP_PLANE4                    = 0x3004;
		public const uint GL_CLIP_PLANE5                    = 0x3005;
	
	    //   DataType
		public const uint GL_BYTE                           = 0x1400;
		public const uint GL_UNSIGNED_BYTE                  = 0x1401;
		public const uint GL_SHORT                          = 0x1402;
		public const uint GL_UNSIGNED_SHORT                 = 0x1403;
		public const uint GL_INT                            = 0x1404;
		public const uint GL_UNSIGNED_INT                   = 0x1405;
		public const uint GL_FLOAT                          = 0x1406;
		public const uint GL_2_BYTES                        = 0x1407;
		public const uint GL_3_BYTES                        = 0x1408;
		public const uint GL_4_BYTES                        = 0x1409;
		public const uint GL_DOUBLE                         = 0x140A;
	
	    //   DrawBufferMode
		public const uint GL_NONE                           = 0;
		public const uint GL_FRONT_LEFT                     = 0x0400;
		public const uint GL_FRONT_RIGHT                    = 0x0401;
		public const uint GL_BACK_LEFT                      = 0x0402;
		public const uint GL_BACK_RIGHT                     = 0x0403;
		public const uint GL_FRONT                          = 0x0404;
		public const uint GL_BACK                           = 0x0405;
		public const uint GL_LEFT                           = 0x0406;
		public const uint GL_RIGHT                          = 0x0407;
		public const uint GL_FRONT_AND_BACK                 = 0x0408;
		public const uint GL_AUX0                           = 0x0409;
		public const uint GL_AUX1                           = 0x040A;
		public const uint GL_AUX2                           = 0x040B;
		public const uint GL_AUX3                           = 0x040C;
	
	    //   ErrorCode
		public const uint GL_NO_ERROR                       = 0;
		public const uint GL_INVALID_ENUM                   = 0x0500;
		public const uint GL_INVALID_VALUE                  = 0x0501;
		public const uint GL_INVALID_OPERATION              = 0x0502;
		public const uint GL_STACK_OVERFLOW                 = 0x0503;
		public const uint GL_STACK_UNDERFLOW                = 0x0504;
		public const uint GL_OUT_OF_MEMORY                  = 0x0505;
	
	    //   FeedBackMode
		public const uint GL_2D                             = 0x0600;
		public const uint GL_3D                             = 0x0601;
		public const uint GL_4D_COLOR                       = 0x0602;
		public const uint GL_3D_COLOR_TEXTURE               = 0x0603;
		public const uint GL_4D_COLOR_TEXTURE               = 0x0604;
	
	    //   FeedBackToken
		public const uint GL_PASS_THROUGH_TOKEN             = 0x0700;
		public const uint GL_POINT_TOKEN                    = 0x0701;
		public const uint GL_LINE_TOKEN                     = 0x0702;
		public const uint GL_POLYGON_TOKEN                  = 0x0703;
		public const uint GL_BITMAP_TOKEN                   = 0x0704;
		public const uint GL_DRAW_PIXEL_TOKEN               = 0x0705;
		public const uint GL_COPY_PIXEL_TOKEN               = 0x0706;
		public const uint GL_LINE_RESET_TOKEN               = 0x0707;
	
	    //   FogMode
	   	public const uint GL_EXP                            = 0x0800;
		public const uint GL_EXP2                           = 0x0801;
	
	    //   FrontFaceDirection
		public const uint GL_CW                             = 0x0900;
		public const uint GL_CCW                            = 0x0901;
	
	    //    GetMapTarget 
		public const uint GL_COEFF                          = 0x0A00;
		public const uint GL_ORDER                          = 0x0A01;
		public const uint GL_DOMAIN                         = 0x0A02;
	
	    //   GetTarget
		public const uint GL_CURRENT_COLOR                  = 0x0B00;
		public const uint GL_CURRENT_INDEX                  = 0x0B01;
		public const uint GL_CURRENT_NORMAL                 = 0x0B02;
		public const uint GL_CURRENT_TEXTURE_COORDS         = 0x0B03;
		public const uint GL_CURRENT_RASTER_COLOR           = 0x0B04;
		public const uint GL_CURRENT_RASTER_INDEX           = 0x0B05;
		public const uint GL_CURRENT_RASTER_TEXTURE_COORDS  = 0x0B06;
		public const uint GL_CURRENT_RASTER_POSITION        = 0x0B07;
		public const uint GL_CURRENT_RASTER_POSITION_VALID  = 0x0B08;
		public const uint GL_CURRENT_RASTER_DISTANCE        = 0x0B09;
		public const uint GL_POINT_SMOOTH                   = 0x0B10;
		public const uint GL_POINT_SIZE                     = 0x0B11;
		public const uint GL_POINT_SIZE_RANGE               = 0x0B12;
		public const uint GL_POINT_SIZE_GRANULARITY         = 0x0B13;
		public const uint GL_LINE_SMOOTH                    = 0x0B20;
		public const uint GL_LINE_WIDTH                     = 0x0B21;
		public const uint GL_LINE_WIDTH_RANGE               = 0x0B22;
		public const uint GL_LINE_WIDTH_GRANULARITY         = 0x0B23;
		public const uint GL_LINE_STIPPLE                   = 0x0B24;
		public const uint GL_LINE_STIPPLE_PATTERN           = 0x0B25;
		public const uint GL_LINE_STIPPLE_REPEAT            = 0x0B26;
		public const uint GL_LIST_MODE                      = 0x0B30;
		public const uint GL_MAX_LIST_NESTING               = 0x0B31;
		public const uint GL_LIST_BASE                      = 0x0B32;
		public const uint GL_LIST_INDEX                     = 0x0B33;
		public const uint GL_POLYGON_MODE                   = 0x0B40;
		public const uint GL_POLYGON_SMOOTH                 = 0x0B41;
		public const uint GL_POLYGON_STIPPLE                = 0x0B42;
		public const uint GL_EDGE_FLAG                      = 0x0B43;
		public const uint GL_CULL_FACE                      = 0x0B44;
		public const uint GL_CULL_FACE_MODE                 = 0x0B45;
		public const uint GL_FRONT_FACE                     = 0x0B46;
		public const uint GL_LIGHTING                       = 0x0B50;
		public const uint GL_LIGHT_MODEL_LOCAL_VIEWER       = 0x0B51;
		public const uint GL_LIGHT_MODEL_TWO_SIDE           = 0x0B52;
		public const uint GL_LIGHT_MODEL_AMBIENT            = 0x0B53;
		public const uint GL_SHADE_MODEL                    = 0x0B54;
		public const uint GL_COLOR_MATERIAL_FACE            = 0x0B55;
		public const uint GL_COLOR_MATERIAL_PARAMETER       = 0x0B56;
		public const uint GL_COLOR_MATERIAL                 = 0x0B57;
		public const uint GL_FOG                            = 0x0B60;
		public const uint GL_FOG_INDEX                      = 0x0B61;
		public const uint GL_FOG_DENSITY                    = 0x0B62;
		public const uint GL_FOG_START                      = 0x0B63;
		public const uint GL_FOG_END                        = 0x0B64;
		public const uint GL_FOG_MODE                       = 0x0B65;
		public const uint GL_FOG_COLOR                      = 0x0B66;
		public const uint GL_DEPTH_RANGE                    = 0x0B70;
		public const uint GL_DEPTH_TEST                     = 0x0B71;
		public const uint GL_DEPTH_WRITEMASK                = 0x0B72;
		public const uint GL_DEPTH_CLEAR_VALUE              = 0x0B73;
		public const uint GL_DEPTH_FUNC                     = 0x0B74;
		public const uint GL_ACCUM_CLEAR_VALUE              = 0x0B80;
		public const uint GL_STENCIL_TEST                   = 0x0B90;
		public const uint GL_STENCIL_CLEAR_VALUE            = 0x0B91;
		public const uint GL_STENCIL_FUNC                   = 0x0B92;
		public const uint GL_STENCIL_VALUE_MASK             = 0x0B93;
		public const uint GL_STENCIL_FAIL                   = 0x0B94;
		public const uint GL_STENCIL_PASS_DEPTH_FAIL        = 0x0B95;
		public const uint GL_STENCIL_PASS_DEPTH_PASS        = 0x0B96;
		public const uint GL_STENCIL_REF                    = 0x0B97;
		public const uint GL_STENCIL_WRITEMASK              = 0x0B98;
		public const uint GL_MATRIX_MODE                    = 0x0BA0;
		public const uint GL_NORMALIZE                      = 0x0BA1;
		public const uint GL_VIEWPORT                       = 0x0BA2;
		public const uint GL_MODELVIEW_STACK_DEPTH          = 0x0BA3;
		public const uint GL_PROJECTION_STACK_DEPTH         = 0x0BA4;
		public const uint GL_TEXTURE_STACK_DEPTH            = 0x0BA5;
		public const uint GL_MODELVIEW_MATRIX               = 0x0BA6;
		public const uint GL_PROJECTION_MATRIX              = 0x0BA7;
		public const uint GL_TEXTURE_MATRIX                 = 0x0BA8;
		public const uint GL_ATTRIB_STACK_DEPTH             = 0x0BB0;
		public const uint GL_CLIENT_ATTRIB_STACK_DEPTH      = 0x0BB1;
		public const uint GL_ALPHA_TEST                     = 0x0BC0;
		public const uint GL_ALPHA_TEST_FUNC                = 0x0BC1;
		public const uint GL_ALPHA_TEST_REF                 = 0x0BC2;
		public const uint GL_DITHER                         = 0x0BD0;
		public const uint GL_BLEND_DST                      = 0x0BE0;
		public const uint GL_BLEND_SRC                      = 0x0BE1;
		public const uint GL_BLEND                          = 0x0BE2;
		public const uint GL_LOGIC_OP_MODE                  = 0x0BF0;
		public const uint GL_INDEX_LOGIC_OP                 = 0x0BF1;
		public const uint GL_COLOR_LOGIC_OP                 = 0x0BF2;
		public const uint GL_AUX_BUFFERS                    = 0x0C00;
		public const uint GL_DRAW_BUFFER                    = 0x0C01;
		public const uint GL_READ_BUFFER                    = 0x0C02;
		public const uint GL_SCISSOR_BOX                    = 0x0C10;
		public const uint GL_SCISSOR_TEST                   = 0x0C11;
		public const uint GL_INDEX_CLEAR_VALUE              = 0x0C20;
		public const uint GL_INDEX_WRITEMASK                = 0x0C21;
		public const uint GL_COLOR_CLEAR_VALUE              = 0x0C22;
		public const uint GL_COLOR_WRITEMASK                = 0x0C23;
		public const uint GL_INDEX_MODE                     = 0x0C30;
		public const uint GL_RGBA_MODE                      = 0x0C31;
		public const uint GL_DOUBLEBUFFER                   = 0x0C32;
		public const uint GL_STEREO                         = 0x0C33;
		public const uint GL_RENDER_MODE                    = 0x0C40;
		public const uint GL_PERSPECTIVE_CORRECTION_HINT    = 0x0C50;
		public const uint GL_POINT_SMOOTH_HINT              = 0x0C51;
		public const uint GL_LINE_SMOOTH_HINT               = 0x0C52;
		public const uint GL_POLYGON_SMOOTH_HINT            = 0x0C53;
		public const uint GL_FOG_HINT                       = 0x0C54;
		public const uint GL_TEXTURE_GEN_S                  = 0x0C60;
		public const uint GL_TEXTURE_GEN_T                  = 0x0C61;
		public const uint GL_TEXTURE_GEN_R                  = 0x0C62;
		public const uint GL_TEXTURE_GEN_Q                  = 0x0C63;
		public const uint GL_PIXEL_MAP_I_TO_I               = 0x0C70;
		public const uint GL_PIXEL_MAP_S_TO_S               = 0x0C71;
		public const uint GL_PIXEL_MAP_I_TO_R               = 0x0C72;
		public const uint GL_PIXEL_MAP_I_TO_G               = 0x0C73;
		public const uint GL_PIXEL_MAP_I_TO_B               = 0x0C74;
		public const uint GL_PIXEL_MAP_I_TO_A               = 0x0C75;
		public const uint GL_PIXEL_MAP_R_TO_R               = 0x0C76;
		public const uint GL_PIXEL_MAP_G_TO_G               = 0x0C77;
		public const uint GL_PIXEL_MAP_B_TO_B               = 0x0C78;
		public const uint GL_PIXEL_MAP_A_TO_A               = 0x0C79;
		public const uint GL_PIXEL_MAP_I_TO_I_SIZE          = 0x0CB0;
		public const uint GL_PIXEL_MAP_S_TO_S_SIZE          = 0x0CB1;
		public const uint GL_PIXEL_MAP_I_TO_R_SIZE          = 0x0CB2;
		public const uint GL_PIXEL_MAP_I_TO_G_SIZE          = 0x0CB3;
		public const uint GL_PIXEL_MAP_I_TO_B_SIZE          = 0x0CB4;
		public const uint GL_PIXEL_MAP_I_TO_A_SIZE          = 0x0CB5;
		public const uint GL_PIXEL_MAP_R_TO_R_SIZE          = 0x0CB6;
		public const uint GL_PIXEL_MAP_G_TO_G_SIZE          = 0x0CB7;
		public const uint GL_PIXEL_MAP_B_TO_B_SIZE          = 0x0CB8;
		public const uint GL_PIXEL_MAP_A_TO_A_SIZE          = 0x0CB9;
		public const uint GL_UNPACK_SWAP_BYTES              = 0x0CF0;
		public const uint GL_UNPACK_LSB_FIRST               = 0x0CF1;
		public const uint GL_UNPACK_ROW_LENGTH              = 0x0CF2;
		public const uint GL_UNPACK_SKIP_ROWS               = 0x0CF3;
		public const uint GL_UNPACK_SKIP_PIXELS             = 0x0CF4;
		public const uint GL_UNPACK_ALIGNMENT               = 0x0CF5;
		public const uint GL_PACK_SWAP_BYTES                = 0x0D00;
		public const uint GL_PACK_LSB_FIRST                 = 0x0D01;
		public const uint GL_PACK_ROW_LENGTH                = 0x0D02;
		public const uint GL_PACK_SKIP_ROWS                 = 0x0D03;
		public const uint GL_PACK_SKIP_PIXELS               = 0x0D04;
		public const uint GL_PACK_ALIGNMENT                 = 0x0D05;
		public const uint GL_MAP_COLOR                      = 0x0D10;
		public const uint GL_MAP_STENCIL                    = 0x0D11;
		public const uint GL_INDEX_SHIFT                    = 0x0D12;
		public const uint GL_INDEX_OFFSET                   = 0x0D13;
		public const uint GL_RED_SCALE                      = 0x0D14;
		public const uint GL_RED_BIAS                       = 0x0D15;
		public const uint GL_ZOOM_X                         = 0x0D16;
		public const uint GL_ZOOM_Y                         = 0x0D17;
		public const uint GL_GREEN_SCALE                    = 0x0D18;
		public const uint GL_GREEN_BIAS                     = 0x0D19;
		public const uint GL_BLUE_SCALE                     = 0x0D1A;
		public const uint GL_BLUE_BIAS                      = 0x0D1B;
		public const uint GL_ALPHA_SCALE                    = 0x0D1C;
		public const uint GL_ALPHA_BIAS                     = 0x0D1D;
		public const uint GL_DEPTH_SCALE                    = 0x0D1E;
		public const uint GL_DEPTH_BIAS                     = 0x0D1F;
		public const uint GL_MAX_EVAL_ORDER                 = 0x0D30;
		public const uint GL_MAX_LIGHTS                     = 0x0D31;
		public const uint GL_MAX_CLIP_PLANES                = 0x0D32;
		public const uint GL_MAX_TEXTURE_SIZE               = 0x0D33;
		public const uint GL_MAX_PIXEL_MAP_TABLE            = 0x0D34;
		public const uint GL_MAX_ATTRIB_STACK_DEPTH         = 0x0D35;
		public const uint GL_MAX_MODELVIEW_STACK_DEPTH      = 0x0D36;
		public const uint GL_MAX_NAME_STACK_DEPTH           = 0x0D37;
		public const uint GL_MAX_PROJECTION_STACK_DEPTH     = 0x0D38;
		public const uint GL_MAX_TEXTURE_STACK_DEPTH        = 0x0D39;
		public const uint GL_MAX_VIEWPORT_DIMS              = 0x0D3A;
		public const uint GL_MAX_CLIENT_ATTRIB_STACK_DEPTH  = 0x0D3B;
		public const uint GL_SUBPIXEL_BITS                  = 0x0D50;
		public const uint GL_INDEX_BITS                     = 0x0D51;
		public const uint GL_RED_BITS                       = 0x0D52;
		public const uint GL_GREEN_BITS                     = 0x0D53;
		public const uint GL_BLUE_BITS                      = 0x0D54;
		public const uint GL_ALPHA_BITS                     = 0x0D55;
		public const uint GL_DEPTH_BITS                     = 0x0D56;
		public const uint GL_STENCIL_BITS                   = 0x0D57;
		public const uint GL_ACCUM_RED_BITS                 = 0x0D58;
		public const uint GL_ACCUM_GREEN_BITS               = 0x0D59;
		public const uint GL_ACCUM_BLUE_BITS                = 0x0D5A;
		public const uint GL_ACCUM_ALPHA_BITS               = 0x0D5B;
		public const uint GL_NAME_STACK_DEPTH               = 0x0D70;
		public const uint GL_AUTO_NORMAL                    = 0x0D80;
		public const uint GL_MAP1_COLOR_4                   = 0x0D90;
		public const uint GL_MAP1_INDEX                     = 0x0D91;
		public const uint GL_MAP1_NORMAL                    = 0x0D92;
		public const uint GL_MAP1_TEXTURE_COORD_1           = 0x0D93;
		public const uint GL_MAP1_TEXTURE_COORD_2           = 0x0D94;
		public const uint GL_MAP1_TEXTURE_COORD_3           = 0x0D95;
		public const uint GL_MAP1_TEXTURE_COORD_4           = 0x0D96;
		public const uint GL_MAP1_VERTEX_3                  = 0x0D97;
		public const uint GL_MAP1_VERTEX_4                  = 0x0D98;
		public const uint GL_MAP2_COLOR_4                   = 0x0DB0;
		public const uint GL_MAP2_INDEX                     = 0x0DB1;
		public const uint GL_MAP2_NORMAL                    = 0x0DB2;
		public const uint GL_MAP2_TEXTURE_COORD_1           = 0x0DB3;
		public const uint GL_MAP2_TEXTURE_COORD_2           = 0x0DB4;
		public const uint GL_MAP2_TEXTURE_COORD_3           = 0x0DB5;
		public const uint GL_MAP2_TEXTURE_COORD_4           = 0x0DB6;
		public const uint GL_MAP2_VERTEX_3                  = 0x0DB7;
		public const uint GL_MAP2_VERTEX_4                  = 0x0DB8;
		public const uint GL_MAP1_GRID_DOMAIN               = 0x0DD0;
		public const uint GL_MAP1_GRID_SEGMENTS             = 0x0DD1;
		public const uint GL_MAP2_GRID_DOMAIN               = 0x0DD2;
		public const uint GL_MAP2_GRID_SEGMENTS             = 0x0DD3;
		public const uint GL_TEXTURE_1D                     = 0x0DE0;
		public const uint GL_TEXTURE_2D                     = 0x0DE1;
		public const uint GL_FEEDBACK_BUFFER_POINTER        = 0x0DF0;
		public const uint GL_FEEDBACK_BUFFER_SIZE           = 0x0DF1;
		public const uint GL_FEEDBACK_BUFFER_TYPE           = 0x0DF2;
		public const uint GL_SELECTION_BUFFER_POINTER       = 0x0DF3;
		public const uint GL_SELECTION_BUFFER_SIZE          = 0x0DF4;
	
	    //   GetTextureParameter
		public const uint GL_TEXTURE_WIDTH                  = 0x1000;
		public const uint GL_TEXTURE_HEIGHT                 = 0x1001;
		public const uint GL_TEXTURE_INTERNAL_FORMAT        = 0x1003;
		public const uint GL_TEXTURE_BORDER_COLOR           = 0x1004;
		public const uint GL_TEXTURE_BORDER                 = 0x1005;
	
	    //   HintMode
		public const uint GL_DONT_CARE                      = 0x1100;
		public const uint GL_FASTEST                        = 0x1101;
		public const uint GL_NICEST                         = 0x1102;
	
	    //   LightName
		public const uint GL_LIGHT0                         = 0x4000;
		public const uint GL_LIGHT1                         = 0x4001;
		public const uint GL_LIGHT2                         = 0x4002;
		public const uint GL_LIGHT3                         = 0x4003;
		public const uint GL_LIGHT4                         = 0x4004;
		public const uint GL_LIGHT5                         = 0x4005;
		public const uint GL_LIGHT6                         = 0x4006;
		public const uint GL_LIGHT7                         = 0x4007;
	
	    //   LightParameter
		public const uint GL_AMBIENT                        = 0x1200;
		public const uint GL_DIFFUSE                        = 0x1201;
		public const uint GL_SPECULAR                       = 0x1202;
		public const uint GL_POSITION                       = 0x1203;
		public const uint GL_SPOT_DIRECTION                 = 0x1204;
		public const uint GL_SPOT_EXPONENT                  = 0x1205;
		public const uint GL_SPOT_CUTOFF                    = 0x1206;
		public const uint GL_CONSTANT_ATTENUATION           = 0x1207;
		public const uint GL_LINEAR_ATTENUATION             = 0x1208;
		public const uint GL_QUADRATIC_ATTENUATION          = 0x1209;
	
	    //   ListMode
		public const uint GL_COMPILE                        = 0x1300;
		public const uint GL_COMPILE_AND_EXECUTE            = 0x1301;
	
	    //   LogicOp
		public const uint GL_CLEAR                          = 0x1500;
		public const uint GL_AND                            = 0x1501;
		public const uint GL_AND_REVERSE                    = 0x1502;
		public const uint GL_COPY                           = 0x1503;
		public const uint GL_AND_INVERTED                   = 0x1504;
		public const uint GL_NOOP                           = 0x1505;
		public const uint GL_XOR                            = 0x1506;
		public const uint GL_OR                             = 0x1507;
		public const uint GL_NOR                            = 0x1508;
		public const uint GL_EQUIV                          = 0x1509;
		public const uint GL_INVERT                         = 0x150A;
		public const uint GL_OR_REVERSE                     = 0x150B;
		public const uint GL_COPY_INVERTED                  = 0x150C;
		public const uint GL_OR_INVERTED                    = 0x150D;
		public const uint GL_NAND                           = 0x150E;
		public const uint GL_SET                            = 0x150F;
	
	    //   MaterialParameter
		public const uint GL_EMISSION                       = 0x1600;
		public const uint GL_SHININESS                      = 0x1601;
		public const uint GL_AMBIENT_AND_DIFFUSE            = 0x1602;
		public const uint GL_COLOR_INDEXES                  = 0x1603;
	
	    //   MatrixMode
		public const uint GL_MODELVIEW                      = 0x1700;
		public const uint GL_PROJECTION                     = 0x1701;
		public const uint GL_TEXTURE                        = 0x1702;
	
	    //   PixelCopyType
		public const uint GL_COLOR                          = 0x1800;
		public const uint GL_DEPTH                          = 0x1801;
		public const uint GL_STENCIL                        = 0x1802;
	
	    //   PixelFormat
		public const uint GL_COLOR_INDEX                    = 0x1900;
		public const uint GL_STENCIL_INDEX                  = 0x1901;
		public const uint GL_DEPTH_COMPONENT                = 0x1902;
		public const uint GL_RED                            = 0x1903;
		public const uint GL_GREEN                          = 0x1904;
		public const uint GL_BLUE                           = 0x1905;
		public const uint GL_ALPHA                          = 0x1906;
		public const uint GL_RGB                            = 0x1907;
		public const uint GL_RGBA                           = 0x1908;
		public const uint GL_LUMINANCE                      = 0x1909;
		public const uint GL_LUMINANCE_ALPHA                = 0x190A;
	
	    //   PixelType
		public const uint GL_BITMAP                     = 0x1A00;
		
	    //   PolygonMode
		public const uint GL_POINT                          = 0x1B00;
		public const uint GL_LINE                           = 0x1B01;
		public const uint GL_FILL                           = 0x1B02;
	
	    //   RenderingMode 
		public const uint GL_RENDER                         = 0x1C00;
		public const uint GL_FEEDBACK                       = 0x1C01;
		public const uint GL_SELECT                         = 0x1C02;
	
	    //   ShadingModel
		public const uint GL_FLAT                           = 0x1D00;
		public const uint GL_SMOOTH                         = 0x1D01;
	
	    //   StencilOp	
		public const uint GL_KEEP                           = 0x1E00;
		public const uint GL_REPLACE                        = 0x1E01;
		public const uint GL_INCR                           = 0x1E02;
		public const uint GL_DECR                           = 0x1E03;
	
	    //   StringName
		public const uint GL_VENDOR                         = 0x1F00;
		public const uint GL_RENDERER                       = 0x1F01;
		public const uint GL_VERSION                        = 0x1F02;
		public const uint GL_EXTENSIONS                     = 0x1F03;
	
	    //   TextureCoordName
		public const uint GL_S                              = 0x2000;
		public const uint GL_T                              = 0x2001;
		public const uint GL_R                              = 0x2002;
		public const uint GL_Q                              = 0x2003;
	
	    //   TextureEnvMode
		public const uint GL_MODULATE                       = 0x2100;
		public const uint GL_DECAL                          = 0x2101;
	
	    //   TextureEnvParameter
		public const uint GL_TEXTURE_ENV_MODE               = 0x2200;
		public const uint GL_TEXTURE_ENV_COLOR              = 0x2201;
	
	    //   TextureEnvTarget
		public const uint GL_TEXTURE_ENV                    = 0x2300;
	
	    //   TextureGenMode 
		public const uint GL_EYE_LINEAR                     = 0x2400;
		public const uint GL_OBJECT_LINEAR                  = 0x2401;
		public const uint GL_SPHERE_MAP                     = 0x2402;
	
	    //   TextureGenParameter
		public const uint GL_TEXTURE_GEN_MODE               = 0x2500;
		public const uint GL_OBJECT_PLANE                   = 0x2501;
		public const uint GL_EYE_PLANE                      = 0x2502;
	
	    //   TextureMagFilter
		public const uint GL_NEAREST                        = 0x2600;
		public const uint GL_LINEAR                         = 0x2601;
	
	    //   TextureMinFilter 
		public const uint GL_NEAREST_MIPMAP_NEAREST         = 0x2700;
		public const uint GL_LINEAR_MIPMAP_NEAREST          = 0x2701;
		public const uint GL_NEAREST_MIPMAP_LINEAR          = 0x2702;
		public const uint GL_LINEAR_MIPMAP_LINEAR           = 0x2703;
	
	    //   TextureParameterName
		public const uint GL_TEXTURE_MAG_FILTER             = 0x2800;
		public const uint GL_TEXTURE_MIN_FILTER             = 0x2801;
		public const uint GL_TEXTURE_WRAP_S                 = 0x2802;
		public const uint GL_TEXTURE_WRAP_T                 = 0x2803;
	
	    //   TextureWrapMode
		public const uint GL_CLAMP                          = 0x2900;
		public const uint GL_REPEAT                         = 0x2901;
	
	    //   ClientAttribMask
		public const uint GL_CLIENT_PIXEL_STORE_BIT         = 0x00000001;
		public const uint GL_CLIENT_VERTEX_ARRAY_BIT        = 0x00000002;
		public const uint GL_CLIENT_ALL_ATTRIB_BITS         = 0xffffffff;
	
	    //   Polygon Offset
		public const uint GL_POLYGON_OFFSET_FACTOR          = 0x8038;
		public const uint GL_POLYGON_OFFSET_UNITS           = 0x2A00;
		public const uint GL_POLYGON_OFFSET_POINT           = 0x2A01;
		public const uint GL_POLYGON_OFFSET_LINE            = 0x2A02;
		public const uint GL_POLYGON_OFFSET_FILL            = 0x8037;
	
	    //   Texture 
		public const uint GL_ALPHA4                         = 0x803B;
		public const uint GL_ALPHA8                         = 0x803C;
		public const uint GL_ALPHA12                        = 0x803D;
		public const uint GL_ALPHA16                        = 0x803E;
		public const uint GL_LUMINANCE4                     = 0x803F;
		public const uint GL_LUMINANCE8                     = 0x8040;
		public const uint GL_LUMINANCE12                    = 0x8041;
		public const uint GL_LUMINANCE16                    = 0x8042;
		public const uint GL_LUMINANCE4_ALPHA4              = 0x8043;
		public const uint GL_LUMINANCE6_ALPHA2              = 0x8044;
		public const uint GL_LUMINANCE8_ALPHA8              = 0x8045;
		public const uint GL_LUMINANCE12_ALPHA4             = 0x8046;
		public const uint GL_LUMINANCE12_ALPHA12            = 0x8047;
		public const uint GL_LUMINANCE16_ALPHA16            = 0x8048;
		public const uint GL_INTENSITY                      = 0x8049;
		public const uint GL_INTENSITY4                     = 0x804A;
		public const uint GL_INTENSITY8                     = 0x804B;
		public const uint GL_INTENSITY12                    = 0x804C;
		public const uint GL_INTENSITY16                    = 0x804D;
		public const uint GL_R3_G3_B2                       = 0x2A10;
		public const uint GL_RGB4                           = 0x804F;
		public const uint GL_RGB5                           = 0x8050;
		public const uint GL_RGB8                           = 0x8051;
		public const uint GL_RGB10                          = 0x8052;
		public const uint GL_RGB12                          = 0x8053;
		public const uint GL_RGB16                          = 0x8054;
		public const uint GL_RGBA2                          = 0x8055;
		public const uint GL_RGBA4                          = 0x8056;
		public const uint GL_RGB5_A1                        = 0x8057;
		public const uint GL_RGBA8                          = 0x8058;
		public const uint GL_RGB10_A2                       = 0x8059;
		public const uint GL_RGBA12                         = 0x805A;
		public const uint GL_RGBA16                         = 0x805B;
		public const uint GL_TEXTURE_RED_SIZE               = 0x805C;
		public const uint GL_TEXTURE_GREEN_SIZE             = 0x805D;
		public const uint GL_TEXTURE_BLUE_SIZE              = 0x805E;
		public const uint GL_TEXTURE_ALPHA_SIZE             = 0x805F;
		public const uint GL_TEXTURE_LUMINANCE_SIZE         = 0x8060;
		public const uint GL_TEXTURE_INTENSITY_SIZE         = 0x8061;
		public const uint GL_PROXY_TEXTURE_1D               = 0x8063;
		public const uint GL_PROXY_TEXTURE_2D               = 0x8064;
	
	    //   Texture object
		public const uint GL_TEXTURE_PRIORITY               = 0x8066;
		public const uint GL_TEXTURE_RESIDENT               = 0x8067;
		public const uint GL_TEXTURE_BINDING_1D             = 0x8068;
		public const uint GL_TEXTURE_BINDING_2D             = 0x8069;
	
	    //   Vertex array
		public const uint GL_VERTEX_ARRAY                   = 0x8074;
		public const uint GL_NORMAL_ARRAY                   = 0x8075;
		public const uint GL_COLOR_ARRAY                    = 0x8076;
		public const uint GL_INDEX_ARRAY                    = 0x8077;
		public const uint GL_TEXTURE_COORD_ARRAY            = 0x8078;
		public const uint GL_EDGE_FLAG_ARRAY                = 0x8079;
		public const uint GL_VERTEX_ARRAY_SIZE              = 0x807A;
		public const uint GL_VERTEX_ARRAY_TYPE              = 0x807B;
		public const uint GL_VERTEX_ARRAY_STRIDE            = 0x807C;
		public const uint GL_NORMAL_ARRAY_TYPE              = 0x807E;
		public const uint GL_NORMAL_ARRAY_STRIDE            = 0x807F;
		public const uint GL_COLOR_ARRAY_SIZE               = 0x8081;
		public const uint GL_COLOR_ARRAY_TYPE               = 0x8082;
		public const uint GL_COLOR_ARRAY_STRIDE             = 0x8083;
		public const uint GL_INDEX_ARRAY_TYPE               = 0x8085;
		public const uint GL_INDEX_ARRAY_STRIDE             = 0x8086;
		public const uint GL_TEXTURE_COORD_ARRAY_SIZE       = 0x8088;
		public const uint GL_TEXTURE_COORD_ARRAY_TYPE       = 0x8089;
		public const uint GL_TEXTURE_COORD_ARRAY_STRIDE     = 0x808A;
		public const uint GL_EDGE_FLAG_ARRAY_STRIDE         = 0x808C;
		public const uint GL_VERTEX_ARRAY_POINTER           = 0x808E;
		public const uint GL_NORMAL_ARRAY_POINTER           = 0x808F;
		public const uint GL_COLOR_ARRAY_POINTER            = 0x8090;
		public const uint GL_INDEX_ARRAY_POINTER            = 0x8091;
		public const uint GL_TEXTURE_COORD_ARRAY_POINTER    = 0x8092;
		public const uint GL_EDGE_FLAG_ARRAY_POINTER        = 0x8093;
		public const uint GL_V2F                            = 0x2A20;
		public const uint GL_V3F                            = 0x2A21;
		public const uint GL_C4UB_V2F                       = 0x2A22;
		public const uint GL_C4UB_V3F                       = 0x2A23;
		public const uint GL_C3F_V3F                        = 0x2A24;
		public const uint GL_N3F_V3F                        = 0x2A25;
		public const uint GL_C4F_N3F_V3F                    = 0x2A26;
		public const uint GL_T2F_V3F                        = 0x2A27;
		public const uint GL_T4F_V4F                        = 0x2A28;
		public const uint GL_T2F_C4UB_V3F                   = 0x2A29;
		public const uint GL_T2F_C3F_V3F                    = 0x2A2A;
		public const uint GL_T2F_N3F_V3F                    = 0x2A2B;
		public const uint GL_T2F_C4F_N3F_V3F                = 0x2A2C;
		public const uint GL_T4F_C4F_N3F_V4F                = 0x2A2D;
	
	//   Extensions
		public const uint GL_EXT_vertex_array               = 1;
		public const uint GL_EXT_bgra                       = 1;
		public const uint GL_EXT_paletted_texture           = 1;
		public const uint GL_WIN_swap_hint                  = 1;
		public const uint GL_WIN_draw_range_elements        = 1;
		
	//   EXT_vertex_array 
		public const uint GL_VERTEX_ARRAY_EXT               = 0x8074;
		public const uint GL_NORMAL_ARRAY_EXT               = 0x8075;
		public const uint GL_COLOR_ARRAY_EXT                = 0x8076;
		public const uint GL_INDEX_ARRAY_EXT                = 0x8077;
		public const uint GL_TEXTURE_COORD_ARRAY_EXT        = 0x8078;
		public const uint GL_EDGE_FLAG_ARRAY_EXT            = 0x8079;
		public const uint GL_VERTEX_ARRAY_SIZE_EXT          = 0x807A;
		public const uint GL_VERTEX_ARRAY_TYPE_EXT          = 0x807B;
		public const uint GL_VERTEX_ARRAY_STRIDE_EXT        = 0x807C;
		public const uint GL_VERTEX_ARRAY_COUNT_EXT         = 0x807D;
		public const uint GL_NORMAL_ARRAY_TYPE_EXT          = 0x807E;
		public const uint GL_NORMAL_ARRAY_STRIDE_EXT        = 0x807F;
		public const uint GL_NORMAL_ARRAY_COUNT_EXT         = 0x8080;
		public const uint GL_COLOR_ARRAY_SIZE_EXT           = 0x8081;
		public const uint GL_COLOR_ARRAY_TYPE_EXT           = 0x8082;
		public const uint GL_COLOR_ARRAY_STRIDE_EXT         = 0x8083;
		public const uint GL_COLOR_ARRAY_COUNT_EXT          = 0x8084;
		public const uint GL_INDEX_ARRAY_TYPE_EXT           = 0x8085;
		public const uint GL_INDEX_ARRAY_STRIDE_EXT         = 0x8086;
		public const uint GL_INDEX_ARRAY_COUNT_EXT          = 0x8087;
		public const uint GL_TEXTURE_COORD_ARRAY_SIZE_EXT   = 0x8088;
		public const uint GL_TEXTURE_COORD_ARRAY_TYPE_EXT   = 0x8089;
		public const uint GL_TEXTURE_COORD_ARRAY_STRIDE_EXT = 0x808A;
		public const uint GL_TEXTURE_COORD_ARRAY_COUNT_EXT  = 0x808B;
		public const uint GL_EDGE_FLAG_ARRAY_STRIDE_EXT     = 0x808C;
		public const uint GL_EDGE_FLAG_ARRAY_COUNT_EXT      = 0x808D;
		public const uint GL_VERTEX_ARRAY_POINTER_EXT       = 0x808E;
		public const uint GL_NORMAL_ARRAY_POINTER_EXT       = 0x808F;
		public const uint GL_COLOR_ARRAY_POINTER_EXT        = 0x8090;
		public const uint GL_INDEX_ARRAY_POINTER_EXT        = 0x8091;
		public const uint GL_TEXTURE_COORD_ARRAY_POINTER_EXT = 0x8092;
		public const uint GL_EDGE_FLAG_ARRAY_POINTER_EXT    = 0x8093;
		public const uint GL_DOUBLE_EXT                     =1;/*DOUBLE*/
		
	//   EXT_paletted_texture
		public const uint GL_COLOR_TABLE_FORMAT_EXT         = 0x80D8;
		public const uint GL_COLOR_TABLE_WIDTH_EXT          = 0x80D9;
		public const uint GL_COLOR_TABLE_RED_SIZE_EXT       = 0x80DA;
		public const uint GL_COLOR_TABLE_GREEN_SIZE_EXT     = 0x80DB;
		public const uint GL_COLOR_TABLE_BLUE_SIZE_EXT      = 0x80DC;
		public const uint GL_COLOR_TABLE_ALPHA_SIZE_EXT     = 0x80DD;
		public const uint GL_COLOR_TABLE_LUMINANCE_SIZE_EXT = 0x80DE;
		public const uint GL_COLOR_TABLE_INTENSITY_SIZE_EXT = 0x80DF;
		public const uint GL_COLOR_INDEX1_EXT               = 0x80E2;
		public const uint GL_COLOR_INDEX2_EXT               = 0x80E3;
		public const uint GL_COLOR_INDEX4_EXT               = 0x80E4;
		public const uint GL_COLOR_INDEX8_EXT               = 0x80E5;
		public const uint GL_COLOR_INDEX12_EXT              = 0x80E6;
		public const uint GL_COLOR_INDEX16_EXT              = 0x80E7;
	
	//   WIN_draw_range_elements
		public const uint GL_MAX_ELEMENTS_VERTICES_WIN      = 0x80E8;
		public const uint GL_MAX_ELEMENTS_INDICES_WIN       = 0x80E9;
	
	//   WIN_phong_shading
		public const uint GL_PHONG_WIN                      = 0x80EA;
		public const uint GL_PHONG_HINT_WIN                 = 0x80EB; 
	

	//   WIN_specular_fog 
		public const uint FOG_SPECULAR_TEXTURE_WIN       = 0x80EC;

        #endregion
        
        #region The GLU DLL Constant Definitions.

        //   Version
		public const uint GLU_VERSION_1_1                 = 1;
		public const uint GLU_VERSION_1_2                 = 1;

		//   Errors: (return value 0 = no error)
		public const uint GLU_INVALID_ENUM        = 100900;
		public const uint GLU_INVALID_VALUE       = 100901;
		public const uint GLU_OUT_OF_MEMORY       = 100902;
		public const uint GLU_INCOMPATIBLE_GL_VERSION    = 100903;

		//   StringName
		public const uint GLU_VERSION             = 100800;
		public const uint GLU_EXTENSIONS          = 100801;

		//   Boolean
		public const uint GLU_TRUE                = 1;
		public const uint GLU_FALSE               = 0;

        //  Quadric constants

		//   QuadricNormal
		public const uint GLU_SMOOTH              = 100000;
		public const uint GLU_FLAT                = 100001;
		public const uint GLU_NONE                = 100002;

		//   QuadricDrawStyle
		public const uint GLU_POINT               = 100010;
		public const uint GLU_LINE                = 100011;
		public const uint GLU_FILL                = 100012;
		public const uint GLU_SILHOUETTE          = 100013;

		//   QuadricOrientation
		public const uint GLU_OUTSIDE             = 100020;
		public const uint GLU_INSIDE              = 100021;

		//  Tesselation constants
		public const double GLU_TESS_MAX_COORD             = 1.0e150;

		//   TessProperty
		public const uint GLU_TESS_WINDING_RULE           =100140;
		public const uint GLU_TESS_BOUNDARY_ONLY          =100141;
		public const uint GLU_TESS_TOLERANCE              =100142;

		//   TessWinding
		public const uint GLU_TESS_WINDING_ODD            =100130;
		public const uint GLU_TESS_WINDING_NONZERO        =100131;
		public const uint GLU_TESS_WINDING_POSITIVE       =100132;
		public const uint GLU_TESS_WINDING_NEGATIVE       =100133;
		public const uint GLU_TESS_WINDING_ABS_GEQ_TWO    =100134;

		//   TessCallback
		public const uint GLU_TESS_BEGIN          =100100;
		public const uint GLU_TESS_VERTEX         =100101;
		public const uint GLU_TESS_END            =100102;
		public const uint GLU_TESS_ERROR          =100103;
		public const uint GLU_TESS_EDGE_FLAG      =100104;
		public const uint GLU_TESS_COMBINE        =100105;
		public const uint GLU_TESS_BEGIN_DATA     =100106;
		public const uint GLU_TESS_VERTEX_DATA    =100107;
		public const uint GLU_TESS_END_DATA       =100108;
		public const uint GLU_TESS_ERROR_DATA     =100109;
		public const uint GLU_TESS_EDGE_FLAG_DATA =100110;
		public const uint GLU_TESS_COMBINE_DATA   =100111;

		//   TessError
		public const uint GLU_TESS_ERROR1     =100151;
		public const uint GLU_TESS_ERROR2     =100152;
		public const uint GLU_TESS_ERROR3     =100153;
		public const uint GLU_TESS_ERROR4     =100154;
		public const uint GLU_TESS_ERROR5     =100155;
		public const uint GLU_TESS_ERROR6     =100156;
		public const uint GLU_TESS_ERROR7     =100157;
		public const uint GLU_TESS_ERROR8     =100158;

		public const uint GLU_TESS_MISSING_BEGIN_POLYGON  =100151;
		public const uint GLU_TESS_MISSING_BEGIN_CONTOUR  =100152;
		public const uint GLU_TESS_MISSING_END_POLYGON    =100153;
		public const uint GLU_TESS_MISSING_END_CONTOUR    =100154;
		public const uint GLU_TESS_COORD_TOO_LARGE        =100155;
		public const uint GLU_TESS_NEED_COMBINE_CALLBACK  =100156;

		//  NURBS constants

		//   NurbsProperty
		public const uint GLU_AUTO_LOAD_MATRIX    =100200;
		public const uint GLU_CULLING             =100201;
		public const uint GLU_SAMPLING_TOLERANCE  =100203;
		public const uint GLU_DISPLAY_MODE        =100204;
		public const uint GLU_PARAMETRIC_TOLERANCE        =100202;
		public const uint GLU_SAMPLING_METHOD             =100205;
		public const uint GLU_U_STEP                      =100206;
		public const uint GLU_V_STEP                      =100207;

		//   NurbsSampling
		public const uint GLU_PATH_LENGTH                 =100215;
		public const uint GLU_PARAMETRIC_ERROR            =100216;
		public const uint GLU_DOMAIN_DISTANCE             =100217;


		//   NurbsTrim
		public const uint GLU_MAP1_TRIM_2         =100210;
		public const uint GLU_MAP1_TRIM_3         =100211;

		//   NurbsDisplay
		//        GLU_FILL                100012
		public const uint GLU_OUTLINE_POLYGON     =100240;
		public const uint GLU_OUTLINE_PATCH       =100241;

		//   NurbsCallback
		//        GLU_ERROR               100103

		//   NurbsErrors
		public const uint GLU_NURBS_ERROR1        =100251;
		public const uint GLU_NURBS_ERROR2        =100252;
		public const uint GLU_NURBS_ERROR3        =100253;
		public const uint GLU_NURBS_ERROR4        =100254;
		public const uint GLU_NURBS_ERROR5        =100255;
		public const uint GLU_NURBS_ERROR6        =100256;
		public const uint GLU_NURBS_ERROR7        =100257;
		public const uint GLU_NURBS_ERROR8        =100258;
		public const uint GLU_NURBS_ERROR9        =100259;
		public const uint GLU_NURBS_ERROR10       =100260;
		public const uint GLU_NURBS_ERROR11       =100261;
		public const uint GLU_NURBS_ERROR12       =100262;
		public const uint GLU_NURBS_ERROR13       =100263;
		public const uint GLU_NURBS_ERROR14       =100264;
		public const uint GLU_NURBS_ERROR15       =100265;
		public const uint GLU_NURBS_ERROR16       =100266;
		public const uint GLU_NURBS_ERROR17       =100267;
		public const uint GLU_NURBS_ERROR18       =100268;
		public const uint GLU_NURBS_ERROR19       =100269;
		public const uint GLU_NURBS_ERROR20       =100270;
		public const uint GLU_NURBS_ERROR21       =100271;
		public const uint GLU_NURBS_ERROR22       =100272;
		public const uint GLU_NURBS_ERROR23       =100273;
		public const uint GLU_NURBS_ERROR24       =100274;
		public const uint GLU_NURBS_ERROR25       =100275;
		public const uint GLU_NURBS_ERROR26       =100276;
		public const uint GLU_NURBS_ERROR27       =100277;
		public const uint GLU_NURBS_ERROR28       =100278;
		public const uint GLU_NURBS_ERROR29       =100279;
		public const uint GLU_NURBS_ERROR30       =100280;
		public const uint GLU_NURBS_ERROR31       =100281;
		public const uint GLU_NURBS_ERROR32       =100282;
		public const uint GLU_NURBS_ERROR33       =100283;
		public const uint GLU_NURBS_ERROR34       =100284;
		public const uint GLU_NURBS_ERROR35       =100285;
		public const uint GLU_NURBS_ERROR36       =100286;
		public const uint GLU_NURBS_ERROR37       =100287;

		#endregion

		#region The OpenGL DLL Functions (Exactly the same naming).

        public const string LIBRARY_OPENGL = "opengl32.dll";

		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glAccum(uint op, float value);

		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glAlphaFunc (uint func, float ref_notkeword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern byte glAreTexturesResident (int n,  uint []textures, byte []residences);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glArrayElement (int i);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glBegin (uint mode);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glBindTexture (uint target, uint texture);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glBitmap (int width, int height, float xorig, float yorig, float xmove, float ymove,  byte []bitmap);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glBlendFunc (uint sfactor, uint dfactor);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glCallList (uint list);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glCallLists (int n, uint type,  IntPtr lists);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glCallLists (int n, uint type,  uint[] lists);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glCallLists (int n, uint type,  byte[] lists);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glClear (uint mask);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glClearAccum (float red, float green, float blue, float alpha);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glClearColor (float red, float green, float blue, float alpha);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glClearDepth (double depth);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glClearIndex (float c);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glClearStencil (int s);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glClipPlane (uint plane,  double []equation);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColor3b (byte red, byte green, byte blue);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColor3bv ( byte []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColor3d (double red, double green, double blue);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColor3dv ( double []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColor3f (float red, float green, float blue);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColor3fv ( float []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColor3i (int red, int green, int blue);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColor3iv ( int []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColor3s (short red, short green, short blue);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColor3sv ( short []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColor3ub (byte red, byte green, byte blue);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColor3ubv ( byte []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColor3ui (uint red, uint green, uint blue);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColor3uiv ( uint []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColor3us (ushort red, ushort green, ushort blue);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColor3usv ( ushort []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColor4b (byte red, byte green, byte blue, byte alpha);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColor4bv ( byte []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColor4d (double red, double green, double blue, double alpha);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColor4dv ( double []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColor4f (float red, float green, float blue, float alpha);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColor4fv ( float []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColor4i (int red, int green, int blue, int alpha);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColor4iv ( int []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColor4s (short red, short green, short blue, short alpha);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColor4sv ( short []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColor4ub (byte red, byte green, byte blue, byte alpha);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColor4ubv ( byte []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColor4ui (uint red, uint green, uint blue, uint alpha);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColor4uiv ( uint []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColor4us (ushort red, ushort green, ushort blue, ushort alpha);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColor4usv ( ushort []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColorMask (byte red, byte green, byte blue, byte alpha);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColorMaterial (uint face, uint mode);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glColorPointer (int size, uint type, int stride,  IntPtr pointer);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glCopyPixels (int x, int y, int width, int height, uint type);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glCopyTexImage1D (uint target, int level, uint internalFormat, int x, int y, int width, int border);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glCopyTexImage2D (uint target, int level, uint internalFormat, int x, int y, int width, int height, int border);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glCopyTexSubImage1D (uint target, int level, int xoffset, int x, int y, int width);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glCopyTexSubImage2D (uint target, int level, int xoffset, int yoffset, int x, int y, int width, int height);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glCullFace (uint mode);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glDeleteLists (uint list, int range);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glDeleteTextures (int n,  uint []textures);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glDepthFunc (uint func);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glDepthMask (byte flag);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glDepthRange (double zNear, double zFar);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glDisable (uint cap);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glDisableClientState (uint array);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glDrawArrays (uint mode, int first, int count);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glDrawBuffer (uint mode);
        [DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glDrawElements(uint mode, int count, uint type, IntPtr indices);
        [DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glDrawElements(uint mode, int count, uint type, uint[] indices);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glDrawPixels(int width, int height, uint format, uint type, float[] pixels);
        [DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glDrawPixels(int width, int height, uint format, uint type, uint[] pixels);
        [DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glDrawPixels(int width, int height, uint format, uint type, ushort[] pixels);
        [DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glDrawPixels(int width, int height, uint format, uint type, byte[] pixels);
        [DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glDrawPixels(int width, int height, uint format, uint type, IntPtr pixels);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glEdgeFlag (byte flag);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glEdgeFlagPointer (int stride,  int[] pointer);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glEdgeFlagv ( byte []flag);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glEnable (uint cap);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glEnableClientState (uint array);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glEnd ();
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glEndList ();
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glEvalCoord1d (double u);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glEvalCoord1dv ( double []u);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glEvalCoord1f (float u);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glEvalCoord1fv ( float []u);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glEvalCoord2d (double u, double v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glEvalCoord2dv ( double []u);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glEvalCoord2f (float u, float v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glEvalCoord2fv ( float []u);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glEvalMesh1 (uint mode, int i1, int i2);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glEvalMesh2 (uint mode, int i1, int i2, int j1, int j2);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glEvalPoint1 (int i);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glEvalPoint2 (int i, int j);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glFeedbackBuffer (int size, uint type, float []buffer);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glFinish ();
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glFlush ();
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glFogf (uint pname, float param);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glFogfv (uint pname,  float []params_notkeyword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glFogi (uint pname, int param);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glFogiv (uint pname,  int []params_notkeyword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glFrontFace (uint mode);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glFrustum (double left, double right, double bottom, double top, double zNear, double zFar);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern uint glGenLists (int range);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glGenTextures (int n, uint []textures);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glGetBooleanv (uint pname, byte []params_notkeyword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glGetClipPlane (uint plane, double []equation);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glGetDoublev (uint pname, double []params_notkeyword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern uint glGetError ();
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glGetFloatv (uint pname, float []params_notkeyword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glGetIntegerv (uint pname, int []params_notkeyword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glGetLightfv (uint light, uint pname, float []params_notkeyword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glGetLightiv (uint light, uint pname, int []params_notkeyword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glGetMapdv (uint target, uint query, double []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glGetMapfv (uint target, uint query, float []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glGetMapiv (uint target, uint query, int []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glGetMaterialfv (uint face, uint pname, float []params_notkeyword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glGetMaterialiv (uint face, uint pname, int []params_notkeyword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glGetPixelMapfv (uint map, float []values);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glGetPixelMapuiv (uint map, uint []values);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glGetPixelMapusv (uint map, ushort []values);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glGetPointerv (uint pname, int[] params_notkeyword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glGetPolygonStipple (byte []mask);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private unsafe static extern sbyte* glGetString (uint name);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glGetTexEnvfv (uint target, uint pname, float []params_notkeyword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glGetTexEnviv (uint target, uint pname, int []params_notkeyword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glGetTexGendv (uint coord, uint pname, double []params_notkeyword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glGetTexGenfv (uint coord, uint pname, float []params_notkeyword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glGetTexGeniv (uint coord, uint pname, int []params_notkeyword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glGetTexImage (uint target, int level, uint format, uint type, int []pixels);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glGetTexLevelParameterfv (uint target, int level, uint pname, float []params_notkeyword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glGetTexLevelParameteriv (uint target, int level, uint pname, int []params_notkeyword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glGetTexParameterfv (uint target, uint pname, float []params_notkeyword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glGetTexParameteriv (uint target, uint pname, int []params_notkeyword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glHint (uint target, uint mode);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glIndexMask (uint mask);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glIndexPointer (uint type, int stride,  int[] pointer);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glIndexd (double c);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glIndexdv ( double []c);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glIndexf (float c);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glIndexfv ( float []c);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glIndexi (int c);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glIndexiv ( int []c);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glIndexs (short c);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glIndexsv ( short []c);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glIndexub (byte c);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glIndexubv ( byte []c);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glInitNames ();
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glInterleavedArrays (uint format, int stride,  int[] pointer);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern byte glIsEnabled (uint cap);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern byte glIsList (uint list);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern byte glIsTexture (uint texture);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glLightModelf (uint pname, float param);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glLightModelfv (uint pname,  float []params_notkeyword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glLightModeli (uint pname, int param);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glLightModeliv (uint pname,  int []params_notkeyword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glLightf (uint light, uint pname, float param);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glLightfv (uint light, uint pname,  float []params_notkeyword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glLighti (uint light, uint pname, int param);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glLightiv (uint light, uint pname,  int []params_notkeyword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glLineStipple (int factor, ushort pattern);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glLineWidth (float width);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glListBase (uint base_notkeyword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glLoadIdentity ();
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glLoadMatrixd ( double []m);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glLoadMatrixf ( float []m);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glLoadName (uint name);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glLogicOp (uint opcode);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glMap1d (uint target, double u1, double u2, int stride, int order,  double []points);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glMap1f (uint target, float u1, float u2, int stride, int order,  float []points);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glMap2d (uint target, double u1, double u2, int ustride, int uorder, double v1, double v2, int vstride, int vorder,  double []points);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glMap2f (uint target, float u1, float u2, int ustride, int uorder, float v1, float v2, int vstride, int vorder,  float []points);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glMapGrid1d (int un, double u1, double u2);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glMapGrid1f (int un, float u1, float u2);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glMapGrid2d (int un, double u1, double u2, int vn, double v1, double v2);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glMapGrid2f (int un, float u1, float u2, int vn, float v1, float v2);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glMaterialf (uint face, uint pname, float param);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glMaterialfv (uint face, uint pname,  float []params_notkeyword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glMateriali (uint face, uint pname, int param);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glMaterialiv (uint face, uint pname,  int []params_notkeyword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glMatrixMode (uint mode);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glMultMatrixd ( double []m);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glMultMatrixf ( float []m);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glNewList (uint list, uint mode);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glNormal3b (byte nx, byte ny, byte nz);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glNormal3bv ( byte []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glNormal3d (double nx, double ny, double nz);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glNormal3dv ( double []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glNormal3f (float nx, float ny, float nz);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glNormal3fv ( float []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glNormal3i (int nx, int ny, int nz);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glNormal3iv ( int []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glNormal3s (short nx, short ny, short nz);
        [DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glNormal3sv(short[] v);
        [DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glNormalPointer(uint type, int stride, IntPtr pointer);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glNormalPointer (uint type, int stride, float[] pointer);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glOrtho (double left, double right, double bottom, double top, double zNear, double zFar);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glPassThrough (float token);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glPixelMapfv (uint map, int mapsize,  float []values);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glPixelMapuiv (uint map, int mapsize,  uint []values);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glPixelMapusv (uint map, int mapsize,  ushort []values);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glPixelStoref (uint pname, float param);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glPixelStorei (uint pname, int param);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glPixelTransferf (uint pname, float param);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glPixelTransferi (uint pname, int param);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glPixelZoom (float xfactor, float yfactor);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glPointSize (float size);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glPolygonMode (uint face, uint mode);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glPolygonOffset (float factor, float units);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glPolygonStipple ( byte []mask);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glPopAttrib ();
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glPopClientAttrib ();
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glPopMatrix ();
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glPopName ();
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glPrioritizeTextures (int n,  uint []textures,  float []priorities);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glPushAttrib (uint mask);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glPushClientAttrib (uint mask);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glPushMatrix ();
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glPushName (uint name);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glRasterPos2d (double x, double y);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glRasterPos2dv ( double []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glRasterPos2f (float x, float y);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glRasterPos2fv ( float []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glRasterPos2i (int x, int y);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glRasterPos2iv ( int []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glRasterPos2s (short x, short y);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glRasterPos2sv ( short []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glRasterPos3d (double x, double y, double z);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glRasterPos3dv ( double []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glRasterPos3f (float x, float y, float z);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glRasterPos3fv ( float []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glRasterPos3i (int x, int y, int z);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glRasterPos3iv ( int []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glRasterPos3s (short x, short y, short z);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glRasterPos3sv ( short []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glRasterPos4d (double x, double y, double z, double w);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glRasterPos4dv ( double []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glRasterPos4f (float x, float y, float z, float w);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glRasterPos4fv ( float []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glRasterPos4i (int x, int y, int z, int w);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glRasterPos4iv ( int []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glRasterPos4s (short x, short y, short z, short w);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glRasterPos4sv ( short []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glReadBuffer (uint mode);
        [DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glReadPixels(int x, int y, int width, int height, uint format, uint type, byte[] pixels);
        [DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glReadPixels(int x, int y, int width, int height, uint format, uint type, IntPtr pixels);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glRectd (double x1, double y1, double x2, double y2);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glRectdv ( double []v1,  double []v2);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glRectf (float x1, float y1, float x2, float y2);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glRectfv ( float []v1,  float []v2);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glRecti (int x1, int y1, int x2, int y2);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glRectiv ( int []v1,  int []v2);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glRects (short x1, short y1, short x2, short y2);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glRectsv ( short []v1,  short []v2);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern int glRenderMode (uint mode);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glRotated (double angle, double x, double y, double z);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glRotatef (float angle, float x, float y, float z);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glScaled (double x, double y, double z);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glScalef (float x, float y, float z);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glScissor (int x, int y, int width, int height);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glSelectBuffer (int size, uint []buffer);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glShadeModel (uint mode);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glStencilFunc (uint func, int ref_notkeword, uint mask);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glStencilMask (uint mask);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glStencilOp (uint fail, uint zfail, uint zpass);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexCoord1d (double s);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexCoord1dv ( double []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexCoord1f (float s);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexCoord1fv ( float []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexCoord1i (int s);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexCoord1iv ( int []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexCoord1s (short s);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexCoord1sv ( short []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexCoord2d (double s, double t);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexCoord2dv ( double []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexCoord2f (float s, float t);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexCoord2fv ( float []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexCoord2i (int s, int t);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexCoord2iv ( int []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexCoord2s (short s, short t);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexCoord2sv ( short []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexCoord3d (double s, double t, double r);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexCoord3dv ( double []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexCoord3f (float s, float t, float r);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexCoord3fv ( float []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexCoord3i (int s, int t, int r);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexCoord3iv ( int []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexCoord3s (short s, short t, short r);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexCoord3sv ( short []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexCoord4d (double s, double t, double r, double q);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexCoord4dv ( double []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexCoord4f (float s, float t, float r, float q);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexCoord4fv ( float []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexCoord4i (int s, int t, int r, int q);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexCoord4iv ( int []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexCoord4s (short s, short t, short r, short q);
        [DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexCoord4sv(short[] v);
        [DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexCoordPointer(int size, uint type, int stride, IntPtr pointer);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexCoordPointer (int size, uint type, int stride,  float[] pointer);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexEnvf (uint target, uint pname, float param);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexEnvfv (uint target, uint pname,  float []params_notkeyword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexEnvi (uint target, uint pname, int param);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexEnviv (uint target, uint pname,  int []params_notkeyword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexGend (uint coord, uint pname, double param);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexGendv (uint coord, uint pname,  double []params_notkeyword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexGenf (uint coord, uint pname, float param);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexGenfv (uint coord, uint pname,  float []params_notkeyword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexGeni (uint coord, uint pname, int param);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexGeniv (uint coord, uint pname,  int []params_notkeyword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexImage1D (uint target, int level, uint internalformat, int width, int border, uint format, uint type,  byte[] pixels);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexImage2D (uint target, int level, uint internalformat, int width, int height, int border, uint format, uint type, byte[] pixels);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexImage2D (uint target, int level, uint internalformat, int width, int height, int border, uint format, uint type, IntPtr pixels);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexParameterf (uint target, uint pname, float param);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexParameterfv (uint target, uint pname,  float []params_notkeyword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexParameteri (uint target, uint pname, int param);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexParameteriv (uint target, uint pname,  int []params_notkeyword);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexSubImage1D (uint target, int level, int xoffset, int width, uint format, uint type,  int[] pixels);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTexSubImage2D (uint target, int level, int xoffset, int yoffset, int width, int height, uint format, uint type,  int[] pixels);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTranslated (double x, double y, double z);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glTranslatef (float x, float y, float z);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glVertex2d (double x, double y);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glVertex2dv ( double []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glVertex2f (float x, float y);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glVertex2fv ( float []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glVertex2i (int x, int y);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glVertex2iv ( int []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glVertex2s (short x, short y);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glVertex2sv ( short []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glVertex3d (double x, double y, double z);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glVertex3dv ( double []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glVertex3f (float x, float y, float z);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glVertex3fv ( float []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glVertex3i (int x, int y, int z);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glVertex3iv ( int []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glVertex3s (short x, short y, short z);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glVertex3sv ( short []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glVertex4d (double x, double y, double z, double w);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glVertex4dv ( double []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glVertex4f (float x, float y, float z, float w);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glVertex4fv ( float []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glVertex4i (int x, int y, int z, int w);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glVertex4iv ( int []v);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glVertex4s (short x, short y, short z, short w);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glVertex4sv ( short []v);
        [DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glVertexPointer(int size, uint type, int stride, IntPtr pointer);
        [DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glVertexPointer(int size, uint type, int stride, short[] pointer);
        [DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glVertexPointer(int size, uint type, int stride, int[] pointer);
        [DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glVertexPointer(int size, uint type, int stride, float[] pointer);
        [DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glVertexPointer(int size, uint type, int stride, double[] pointer);
		[DllImport(LIBRARY_OPENGL, SetLastError = true)] private static extern void glViewport (int x, int y, int width, int height);
	
        #endregion

		#region The GLU DLL Functions (Exactly the same naming).

        internal const string LIBRARY_GLU = "Glu32.dll";

		[DllImport(LIBRARY_GLU, SetLastError = true)] private static unsafe extern sbyte* gluErrorString(uint errCode);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static unsafe extern sbyte* gluGetString(int name);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void gluOrtho2D(double left, double right, double bottom, double top);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void gluPerspective (double fovy, double aspect, double zNear, double zFar);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void gluPickMatrix ( double x, double y, double width, double height, int[] viewport);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void gluLookAt ( double eyex, double eyey, double eyez, double centerx, double centery, double centerz, double upx, double upy, double upz);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void gluProject (double objx, double        objy, double        objz,   double[]  modelMatrix,  double[]  projMatrix,  int[] viewport, double [] winx, double        []winy, double        []winz);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void gluUnProject (double winx, double winy, double winz, double[] modelMatrix, double[] projMatrix, int[] viewport, ref double objx, ref double objy, ref double objz);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void gluScaleImage (int format, int widthin, int heightin,  int typein,  int  []datain, int       widthout, int       heightout, int      typeout, int[] dataout);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void gluBuild1DMipmaps (uint target, uint components, int width, uint format, uint type,  IntPtr data);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void gluBuild2DMipmaps (uint target, uint components, int width, int height, uint format, uint type, IntPtr data);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern IntPtr gluNewQuadric();
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void gluDeleteQuadric (IntPtr state);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void gluQuadricNormals (IntPtr quadObject, uint normals);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void gluQuadricTexture (IntPtr quadObject, int textureCoords);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void gluQuadricOrientation (IntPtr quadObject, int orientation);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void gluQuadricDrawStyle (IntPtr quadObject, uint drawStyle);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void gluCylinder(IntPtr           qobj,double            baseRadius, double topRadius, double height,int slices,int stacks);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void gluDisk(IntPtr qobj, double innerRadius,double outerRadius,int slices, int loops);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void gluPartialDisk(IntPtr qobj,double innerRadius,double outerRadius, int slices, int loops, double startAngle, double sweepAngle);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void gluSphere(IntPtr qobj, double radius, int slices, int stacks);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern IntPtr gluNewTess();
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void  gluDeleteTess(IntPtr tess);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void  gluTessBeginPolygon(IntPtr tess, IntPtr polygonData);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void  gluTessBeginContour(IntPtr tess);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void  gluTessVertex(IntPtr tess,double[] coords, double[] data );
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void  gluTessEndContour(   IntPtr        tess );
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void  gluTessEndPolygon(   IntPtr        tess );
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void  gluTessProperty(     IntPtr        tess,int              which, double            value );
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void  gluTessNormal(       IntPtr        tess, double            x,double            y, double            z );
//		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void  gluTessCallback(IntPtr tess, int which, SharpGL.Delegates.Tesselators.Begin callback);
//		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void  gluTessCallback(IntPtr tess, int which, SharpGL.Delegates.Tesselators.BeginData callback);
//		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void  gluTessCallback(IntPtr tess, int which, SharpGL.Delegates.Tesselators.Combine callback);
//		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void  gluTessCallback(IntPtr tess, int which, SharpGL.Delegates.Tesselators.CombineData callback);
//		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void  gluTessCallback(IntPtr tess, int which, SharpGL.Delegates.Tesselators.EdgeFlag callback);
//		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void  gluTessCallback(IntPtr tess, int which, SharpGL.Delegates.Tesselators.EdgeFlagData callback);
//		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void  gluTessCallback(IntPtr tess, int which, SharpGL.Delegates.Tesselators.End callback);
//		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void  gluTessCallback(IntPtr tess, int which, SharpGL.Delegates.Tesselators.EndData callback);
//		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void  gluTessCallback(IntPtr tess, int which, SharpGL.Delegates.Tesselators.Error callback);
//		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void  gluTessCallback(IntPtr tess, int which, SharpGL.Delegates.Tesselators.ErrorData callback);
//		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void  gluTessCallback(IntPtr tess, int which, SharpGL.Delegates.Tesselators.Vertex callback);
//		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void  gluTessCallback(IntPtr tess, int which, SharpGL.Delegates.Tesselators.VertexData callback);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void  gluGetTessProperty(  IntPtr        tess,int              which, double            value );
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern IntPtr gluNewNurbsRenderer ();
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void gluDeleteNurbsRenderer (IntPtr            nobj);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void gluBeginSurface (IntPtr            nobj);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void gluBeginCurve (IntPtr            nobj);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void gluEndCurve (IntPtr            nobj);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void gluEndSurface (IntPtr            nobj);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void gluBeginTrim (IntPtr            nobj);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void gluEndTrim (IntPtr            nobj);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void gluPwlCurve (IntPtr            nobj, int               count, float             array, int stride, uint type);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void gluNurbsCurve(IntPtr nobj, int nknots, float[] knot, int               stride, float[] ctlarray, int               order, uint type);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void gluNurbsSurface(IntPtr nobj, int sknot_count, float[] sknot, int tknot_count, float[]             tknot, int               s_stride, int               t_stride, float[] ctlarray, int sorder, int               torder, uint              type);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void gluLoadSamplingMatrices (IntPtr            nobj,  float[] modelMatrix,  float[] projMatrix, int[] viewport);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void gluNurbsProperty(IntPtr nobj, int property, float value);
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void gluGetNurbsProperty (IntPtr            nobj, int              property, float             value );
		[DllImport(LIBRARY_GLU, SetLastError = true)] private static extern void IntPtrCallback(IntPtr            nobj, int              which, IntPtr Callback );

		#endregion

		#region Wrapped OpenGL Functions

		/// <summary>
		/// Set the Accumulation Buffer operation.
		/// </summary>
		/// <param name="op">Operation of the buffer.</param>
		/// <param name="value">Reference value.</param>
		 public static void Accum(uint op, float value)
		{
			PreGLCall();
			glAccum(op, value);
			PostGLCall();
		}

        /// <summary>
        /// Set the Accumulation Buffer operation.
        /// </summary>
        /// <param name="op">Operation of the buffer.</param>
        /// <param name="value">Reference value.</param>
         public static void Accum(AccumOperation op, float value)
        {
            PreGLCall();
            glAccum((uint)op, value);
            PostGLCall();
        }

        /// <summary>
        /// Specify the Alpha Test function.
        /// </summary>
        /// <param name="func">Specifies the alpha comparison function. Symbolic constants OpenGL.NEVER, OpenGL.LESS, OpenGL.EQUAL, OpenGL.LEQUAL, OpenGL.GREATER, OpenGL.NOTEQUAL, OpenGL.GEQUAL and OpenGL.ALWAYS are accepted. The initial value is OpenGL.ALWAYS.</param>
        /// <param name="reference">Specifies the reference	value that incoming alpha values are compared to. This value is clamped to the range 0	through	1, where 0 represents the lowest possible alpha value and 1 the highest possible value. The initial reference value is 0.</param>
		 public static void AlphaFunc(uint func, float reference)
        {
            PreGLCall();
            glAlphaFunc(func, reference);
            PostGLCall();
        }

        /// <summary>
        /// Specify the Alpha Test function.
        /// </summary>
        /// <param name="function">Specifies the alpha comparison function.</param>
        /// <param name="reference">Specifies the reference	value that incoming alpha values are compared to. This value is clamped to the range 0	through	1, where 0 represents the lowest possible alpha value and 1 the highest possible value. The initial reference value is 0.</param>
         public static void AlphaFunc(AlphaTestFunction function, float reference)
        {
            PreGLCall();
            glAlphaFunc((uint)function, reference);
            PostGLCall();
        }

        /// <summary>
        /// Determine if textures are loaded in texture memory.
        /// </summary>
        /// <param name="n">Specifies the number of textures to be queried.</param>
        /// <param name="textures">Specifies an array containing the names of the textures to be queried.</param>
        /// <param name="residences">Specifies an array in which the texture residence status is returned. The residence status of a texture named by an element of textures is returned in the corresponding element of residences.</param>
        /// <returns></returns>
		public static byte AreTexturesResident(int n,  uint []textures, byte []residences)
        {
            PreGLCall();
            byte returnValue = glAreTexturesResident(n, textures, residences);
            PostGLCall();

            return returnValue;
        }

        /// <summary>
        /// Render a vertex using the specified vertex array element.
        /// </summary>
        /// <param name="i">Specifies an index	into the enabled vertex	data arrays.</param>
		 public static void ArrayElement(int i)
        {
            PreGLCall();
            glArrayElement(i);
            PostGLCall();
        }
        
		/// <summary>
		/// Begin drawing geometry in the specified mode.
		/// </summary>
		/// <param name="mode">The mode to draw in, e.g. OpenGL.POLYGONS.</param>
         public static void Begin(uint mode)
        {
            // Do PreGLCall now, and PostGLCall AFTER End()
            PreGLCall();

            //  Let's remember something important here - you CANNOT call 'glGetError'
            //  between glBegin and glEnd. So we set the 'begun' flag - this'll
            //  turn off error reporting until glEnd.
            glBegin(mode);

            //  Set the begun flag.
            insideGLBegin = true;
        }

        /// <summary>
        /// Begin drawing geometry in the specified mode.
        /// </summary>
        /// <param name="mode">The mode to draw in, e.g. OpenGL.POLYGONS.</param>
         public static void Begin(PrimitiveType mode)
        {
            // Do PreGLCall now, and PostGLCall AFTER End()
            PreGLCall();

            //  Let's remember something important here - you CANNOT call 'glGetError'
            //  between glBegin and glEnd. So we set the 'begun' flag - this'll
            //  turn off error reporting until glEnd.
            glBegin((uint)mode);

            //  Set the begun flag.
            insideGLBegin = true;
        }

		/// <summary>
		/// This function begins drawing a NURBS curve.
		/// </summary>
		/// <param name="nurbsObject">The NURBS object.</param>
         public static void BeginCurve(IntPtr nurbsObject)
		{
			PreGLCall();
			gluBeginCurve(nurbsObject);
			PostGLCall();
		}

		/// <summary>
		/// This function begins drawing a NURBS surface.
		/// </summary>
		/// <param name="nurbsObject">The NURBS object.</param>
		 public static void BeginSurface(IntPtr nurbsObject)
		{
			PreGLCall();
			gluBeginSurface(nurbsObject);
			PostGLCall();
		}

		/// <summary>
		/// Call this function after creating a texture to finalise creation of it, 
		/// or to make an existing texture current.
		/// </summary>
		/// <param name="target">The target type, e.g TEXTURE_2D.</param>
		/// <param name="texture">The OpenGL texture object.</param>
		 public static void BindTexture(uint target, uint texture)
		{
			PreGLCall();
			glBindTexture(target, texture);
			PostGLCall();
		}

        /// <summary>
        /// Draw a bitmap.
        /// </summary>
        /// <param name="width">Specify the pixel width	of the bitmap image.</param>
        /// <param name="height">Specify the pixel height of the bitmap image.</param>
        /// <param name="xorig">Specify	the location of	the origin in the bitmap image. The origin is measured from the lower left corner of the bitmap, with right and up being the positive axes.</param>
        /// <param name="yorig">Specify	the location of	the origin in the bitmap image. The origin is measured from the lower left corner of the bitmap, with right and up being the positive axes.</param>
        /// <param name="xmove">Specify	the x and y offsets to be added	to the current	raster position	after the bitmap is drawn.</param>
        /// <param name="ymove">Specify	the x and y offsets to be added	to the current	raster position	after the bitmap is drawn.</param>
        /// <param name="bitmap">Specifies the address of the bitmap image.</param>
		 public static void Bitmap(int width, int height, float xorig, float yorig, float xmove, float ymove,  byte []bitmap)
        {
            PreGLCall();
            glBitmap(width, height, xorig, yorig, xmove, ymove, bitmap);
            PostGLCall();
        }

		/// <summary>
		/// This function sets the current blending function.
		/// </summary>
		/// <param name="sfactor">Source factor.</param>
		/// <param name="dfactor">Destination factor.</param>
		 public static void BlendFunc(uint sfactor, uint dfactor)
		{
			PreGLCall();
			glBlendFunc(sfactor,dfactor);
			PostGLCall();
		}

        /// <summary>
        /// This function sets the current blending function.
        /// </summary>
        /// <param name="sourceFactor">The source factor.</param>
        /// <param name="destinationFactor">The destination factor.</param>
         public static void BlendFunc(BlendingSourceFactor sourceFactor, BlendingDestinationFactor destinationFactor)
        {
            PreGLCall();
            glBlendFunc((uint)sourceFactor, (uint)destinationFactor);
            PostGLCall();
        }

		/// <summary>
		/// This function calls a certain display list.
		/// </summary>
		/// <param name="list">The display list to call.</param>
		 public static void CallList(uint list)
		{
			PreGLCall();
			glCallList(list);
			PostGLCall();
		}

        /// <summary>
        /// Execute	a list of display lists.
        /// </summary>
        /// <param name="n">Specifies the number of display lists to be executed.</param>
        /// <param name="type">Specifies the type of values in lists. Symbolic constants OpenGL.BYTE, OpenGL.UNSIGNED_BYTE, OpenGL.SHORT, OpenGL.UNSIGNED_SHORT, OpenGL.INT, OpenGL.UNSIGNED_INT, OpenGL.FLOAT, OpenGL.2_BYTES, OpenGL.3_BYTES and OpenGL.4_BYTES are accepted.</param>
        /// <param name="lists">Specifies the address of an array of name offsets in the display list. The pointer type is void because the offsets can be bytes, shorts, ints, or floats, depending on the value of type.</param>
		 public static void CallLists (int n, uint type, IntPtr lists)
        {
            PreGLCall();
            glCallLists(n, type, lists);
            PostGLCall();
        }

        /// <summary>
        /// Execute	a list of display lists.
        /// </summary>
        /// <param name="n">Specifies the number of display lists to be executed.</param>
        /// <param name="type">Specifies the type of values in lists. Symbolic constants OpenGL.BYTE, OpenGL.UNSIGNED_BYTE, OpenGL.SHORT, OpenGL.UNSIGNED_SHORT, OpenGL.INT, OpenGL.UNSIGNED_INT, OpenGL.FLOAT, OpenGL.2_BYTES, OpenGL.3_BYTES and OpenGL.4_BYTES are accepted.</param>
        /// <param name="lists">Specifies the address of an array of name offsets in the display list. The pointer type is void because the offsets can be bytes, shorts, ints, or floats, depending on the value of type.</param>
         public static void CallLists(int n, DataType type, IntPtr lists)
        {
            PreGLCall();
            glCallLists(n, (uint)type, lists);
            PostGLCall();
        }

        /// <summary>
        /// Execute	a list of display lists. Automatically uses the GL_UNSIGNED_BYTE version of the function.
        /// </summary>
        /// <param name="n">The number of lists.</param>
        /// <param name="lists">The lists.</param>
         public static void CallLists(int n, byte[] lists)
        {
            PreGLCall();
            glCallLists(n, GL_UNSIGNED_BYTE, lists);
            PostGLCall();
        }

        /// <summary>
        /// Execute	a list of display lists. Automatically uses the GL_UNSIGNED_INT version of the function.
        /// </summary>
        /// <param name="n">The number of lists.</param>
        /// <param name="lists">The lists.</param>
         public static void CallLists(int n, uint[] lists)
        {
            PreGLCall();
            glCallLists(n, GL_UNSIGNED_INT, lists);
            PostGLCall();
        }

		/// <summary>
		/// This function clears the buffers specified by mask.
		/// </summary>
		/// <param name="mask">Which buffers to clear.</param>
		 public static void Clear(ClearBufferMask mask)
		{
			PreGLCall();
			glClear((uint)mask);
			PostGLCall();
		}

        /// <summary>
        /// Specify clear values for the accumulation buffer.
        /// </summary>
        /// <param name="red">Specify the red, green, blue and alpha values used when the accumulation buffer is cleared. The initial values are all 0.</param>
        /// <param name="green">Specify the red, green, blue and alpha values used when the accumulation buffer is cleared. The initial values are all 0.</param>
        /// <param name="blue">Specify the red, green, blue and alpha values used when the accumulation buffer is cleared. The initial values are all 0.</param>
        /// <param name="alpha">Specify the red, green, blue and alpha values used when the accumulation buffer is cleared. The initial values are all 0.</param>
		 public static void ClearAccum (float red, float green, float blue, float alpha)
        {
            PreGLCall();
            glClearAccum(red, green, blue, alpha);
            PostGLCall();
        }

		/// <summary>
		/// This function sets the color that the drawing buffer is 'cleared' to.
		/// </summary>
		/// <param name="red">Red component of the color (between 0 and 1).</param>
		/// <param name="green">Green component of the color (between 0 and 1).</param>
		/// <param name="blue">Blue component of the color (between 0 and 1)./</param>
		/// <param name="alpha">Alpha component of the color (between 0 and 1).</param>
		 public static void ClearColor (float red, float green, float blue, float alpha)
		{
			PreGLCall();
			glClearColor(red, green, blue, alpha);
			PostGLCall();
		}

        /// <summary>
        /// Specify the clear value for the depth buffer.
        /// </summary>
        /// <param name="depth">Specifies the depth value used	when the depth buffer is cleared. The initial value is 1.</param>
		 public static void ClearDepth(double depth)
        {
            PreGLCall();
            glClearDepth(depth);
            PostGLCall();
        }

        /// <summary>
        /// Specify the clear value for the color index buffers.
        /// </summary>
        /// <param name="c">Specifies the index used when the color index buffers are cleared. The initial value is 0.</param>
		 public static void ClearIndex (float c)
        {
            PreGLCall();
            glClearIndex(c);
            PostGLCall();
        }

        /// <summary>
        /// Specify the clear value for the stencil buffer.
        /// </summary>
        /// <param name="s">Specifies the index used when the stencil buffer is cleared. The initial value is 0.</param>
		 public static void ClearStencil (int s)
        {
            PreGLCall();
            glClearStencil(s);
            PostGLCall();
        }

        /// <summary>
        /// Specify a plane against which all geometry is clipped.
        /// </summary>
        /// <param name="plane">Specifies which clipping plane is being positioned. Symbolic names of the form OpenGL.CLIP_PLANEi, where i is an integer between 0 and OpenGL.MAX_CLIP_PLANES -1, are accepted.</param>
        /// <param name="equation">Specifies the address of an	array of four double-precision floating-point values. These values are interpreted as a plane equation.</param>
		 public static void ClipPlane (uint plane,  double []equation)
        {
            PreGLCall();
            glClipPlane(plane, equation);
            PostGLCall();
        }

        /// <summary>
        /// Specify a plane against which all geometry is clipped.
        /// </summary>
        /// <param name="plane">Specifies which clipping plane is being positioned. Symbolic names of the form OpenGL.CLIP_PLANEi, where i is an integer between 0 and OpenGL.MAX_CLIP_PLANES -1, are accepted.</param>
        /// <param name="equation">Specifies the address of an	array of four double-precision floating-point values. These values are interpreted as a plane equation.</param>
         public static void ClipPlane(ClipPlaneName plane, double[] equation)
        {
            PreGLCall();
            glClipPlane((uint)plane, equation);
            PostGLCall();
        }

		/// <summary>
		/// Sets the current color.
		/// </summary>
		/// <param name="red">Red color component (between 0 and 255).</param>
		/// <param name="green">Green color component (between 0 and 255).</param>
		/// <param name="blue">Blue color component (between 0 and 255).</param>
		 public static void Color(byte red, byte green, byte blue)
		{
			PreGLCall();
			glColor3ub(red, green, blue);
			PostGLCall();
		}

        /// <summary>
        /// Sets the current color.
        /// </summary>
        /// <param name="red">Red color component (between 0 and 255).</param>
        /// <param name="green">Green color component (between 0 and 255).</param>
        /// <param name="blue">Blue color component (between 0 and 255).</param>
        /// <param name="alpha">Alpha color component (between 0 and 255).</param>
         public static void Color(byte red, byte green, byte blue, byte alpha)
        {
            PreGLCall();
            glColor4ub(red, green, blue, alpha);
            PostGLCall();
        }

		/// <summary>
		/// Sets the current color.
		/// </summary>
		/// <param name="red">Red color component (between 0 and 1).</param>
		/// <param name="green">Green color component (between 0 and 1).</param>
		/// <param name="blue">Blue color component (between 0 and 1).</param>
		 public static void Color(double red, double green, double blue)
		{
			PreGLCall();
			glColor3d(red, green, blue);
			PostGLCall();
		}

        /// <summary>
        /// Sets the current color.
        /// </summary>
        /// <param name="red">Red color component (between 0 and 1).</param>
        /// <param name="green">Green color component (between 0 and 1).</param>
        /// <param name="blue">Blue color component (between 0 and 1).</param>
        /// <param name="alpha">Alpha color component.</param>
         public static void Color(double red, double green, double blue, double alpha)
        {
            PreGLCall();
            glColor4d(red, green, blue, alpha);
            PostGLCall();
        }

		/// <summary>
		/// Sets the current color.
		/// </summary>
		/// <param name="red">Red color component (between 0 and 1).</param>
		/// <param name="green">Green color component (between 0 and 1).</param>
		/// <param name="blue">Blue color component (between 0 and 1).</param>
		 public static void Color(float red, float green, float blue)
		{
			PreGLCall();
			glColor3f(red, green, blue);
			PostGLCall();
		}

		/// <summary>
		/// Sets the current color to 'v'.
		/// </summary>
		/// <param name="v">An array of either 3 or 4 float values.</param>
		 public static void Color(float[] v)
		{
			PreGLCall();
			if(v.Length == 3)
				glColor3fv(v);
			else if(v.Length == 4)
				glColor4fv(v);
			PostGLCall();
		}

        /// <summary>
        /// Sets the current color to 'v'.
        /// </summary>
        /// <param name="v">An array of either 3 or 4 int values.</param>
         public static void Color(int[] v)
        {
            PreGLCall();
            if (v.Length == 3)
                glColor3iv(v);
            else if (v.Length == 4)
                glColor4iv(v);
            PostGLCall();
        }

        /// <summary>
        /// Sets the current color to 'v'.
        /// </summary>
        /// <param name="v">An array of either 3 or 4 int values.</param>
         public static void Color(short[] v)
        {
            PreGLCall();
            if (v.Length == 3)
                glColor3sv(v);
            else if (v.Length == 4)
                glColor4sv(v);
            PostGLCall();
        }

        /// <summary>
        /// Sets the current color to 'v'.
        /// </summary>
        /// <param name="v">An array of either 3 or 4 double values.</param>
         public static void Color(double[] v)
        {
            PreGLCall();
            if (v.Length == 3)
                glColor3dv(v);
            else if (v.Length == 4)
                glColor4dv(v);
            PostGLCall();
        }

        /// <summary>
        /// Sets the current color to 'v'.
        /// </summary>
        /// <param name="v">An array of either 3 or 4 byte values.</param>
         public static void Color(byte[] v)
        {
            PreGLCall();
            if (v.Length == 3)
                glColor3bv(v);
            else if (v.Length == 4)
                glColor4bv(v);
            PostGLCall();
        }

        /// <summary>
        /// Sets the current color to 'v'.
        /// </summary>
        /// <param name="v">An array of either 3 or 4 unsigned int values.</param>
         public static void Color(uint[] v) 
        {
            PreGLCall();
            if (v.Length == 3)
                glColor3uiv(v);
            else if (v.Length == 4)
                glColor4uiv(v);
            PostGLCall();
        }

        /// <summary>
        /// Sets the current color to 'v'.
        /// </summary>
        /// <param name="v">An array of either 3 or 4 unsigned short values.</param>
         public static void Color(ushort[] v)
        {
            PreGLCall();
            if (v.Length == 3)
                glColor3usv(v);
            else if (v.Length == 4)
                glColor4usv(v);
            PostGLCall();
        }

		/// <summary>
		/// Sets the current color.
		/// </summary>
		/// <param name="red">Red color component (between 0 and 1).</param>
		/// <param name="green">Green color component (between 0 and 1).</param>
		/// <param name="blue">Blue color component (between 0 and 1).</param>
		 public static void Color(int red, int green, int blue)
		{
			PreGLCall();
			glColor3i(red, green, blue);
			PostGLCall();
		}

        /// <summary>
        /// Sets the current color.
        /// </summary>
        /// <param name="red">Red color component (between 0 and 1).</param>
        /// <param name="green">Green color component (between 0 and 1).</param>
        /// <param name="blue">Blue color component (between 0 and 1).</param>
        /// <param name="alpha">Alpha color component.</param>
         public static void Color(int red, int green, int blue, int alpha)
        {
            PreGLCall();
            glColor4i(red, green, blue, alpha);
            PostGLCall();
        }

		/// <summary>
		/// Sets the current color.
		/// </summary>
		/// <param name="red">Red color component (between 0 and 1).</param>
		/// <param name="green">Green color component (between 0 and 1).</param>
		/// <param name="blue">Blue color component (between 0 and 1).</param>
		 public static void Color(short red, short green, short blue)
		{
			PreGLCall();
			glColor3s(red, green, blue);
			PostGLCall();
		}

        /// <summary>
        /// Sets the current color.
        /// </summary>
        /// <param name="red">Red color component (between 0 and 1).</param>
        /// <param name="green">Green color component (between 0 and 1).</param>
        /// <param name="blue">Blue color component (between 0 and 1).</param>
        /// <param name="alpha">Alpha color component.</param>
         public static void Color(short red, short green, short blue, short alpha)
        {
            PreGLCall();
            glColor4s(red, green, blue, alpha);
            PostGLCall();
        }

		/// <summary>
		/// Sets the current color.
		/// </summary>
		/// <param name="red">Red color component (between 0 and 1).</param>
		/// <param name="green">Green color component (between 0 and 1).</param>
		/// <param name="blue">Blue color component (between 0 and 1).</param>
		 public static void Color(uint red, uint green, uint blue)
		{
			PreGLCall();
			glColor3ui(red, green, blue);
			PostGLCall();
		}

        /// <summary>
        /// Sets the current color.
        /// </summary>
        /// <param name="red">Red color component (between 0 and 1).</param>
        /// <param name="green">Green color component (between 0 and 1).</param>
        /// <param name="blue">Blue color component (between 0 and 1).</param>
        /// <param name="alpha">Alpha color component.</param>
         public static void Color(uint red, uint green, uint blue, uint alpha)
        {
            PreGLCall();
            glColor4ui(red, green, blue, alpha);
            PostGLCall();
        }

		/// <summary>
		/// Sets the current color.
		/// </summary>
		/// <param name="red">Red color component (between 0 and 1).</param>
		/// <param name="green">Green color component (between 0 and 1).</param>
		/// <param name="blue">Blue color component (between 0 and 1).</param>
		 public static void Color(ushort red, ushort green, ushort blue)
		{
			PreGLCall();
			glColor3us(red, green, blue);
			PostGLCall();
		}

        /// <summary>
        /// Sets the current color.
        /// </summary>
        /// <param name="red">Red color component (between 0 and 1).</param>
        /// <param name="green">Green color component (between 0 and 1).</param>
        /// <param name="blue">Blue color component (between 0 and 1).</param>
        /// <param name="alpha">Alpha color component.</param>
         public static void Color(ushort red, ushort green, ushort blue, ushort alpha)
        {
            PreGLCall();
            glColor4us(red, green, blue, alpha);
            PostGLCall();
        }

		/// <summary>
		/// Sets the current color.
		/// </summary>
		/// <param name="red">Red color component (between 0 and 1).</param>
		/// <param name="green">Green color component (between 0 and 1).</param>
		/// <param name="blue">Blue color component (between 0 and 1).</param>
		/// <param name="alpha">Alpha color component (between 0 and 1).</param>
		 public static void Color(float red, float green, float blue, float alpha)
		{
			PreGLCall();
			glColor4f(red, green, blue, alpha);
			PostGLCall();
		}

		/// <summary>
		/// This function sets the current colour mask.
		/// </summary>
		/// <param name="red">Red component mask.</param>
		/// <param name="green">Green component mask.</param>
		/// <param name="blue">Blue component mask.</param>
		/// <param name="alpha">Alpha component mask.</param>
		 public static void ColorMask(byte red, byte green, byte blue, byte alpha)
		{
			PreGLCall();
			glColorMask(red, green, blue, alpha);
			PostGLCall();
		}

        /// <summary>
        /// Cause a material color to track the current color.
        /// </summary>
        /// <param name="face">Specifies whether front, back, or both front and back material parameters should track the current color. Accepted values are OpenGL.FRONT, OpenGL.BACK, and OpenGL.FRONT_AND_BACK. The initial value is OpenGL.FRONT_AND_BACK.</param>
        /// <param name="mode">Specifies which	of several material parameters track the current color. Accepted values are	OpenGL.EMISSION, OpenGL.AMBIENT, OpenGL.DIFFUSE, OpenGL.SPECULAR and OpenGL.AMBIENT_AND_DIFFUSE. The initial value is OpenGL.AMBIENT_AND_DIFFUSE.</param>
		 public static void ColorMaterial (uint face, uint mode)
        {
            PreGLCall();
            glColorMaterial(face, mode);
            PostGLCall();
        }

        /// <summary>
        /// Define an array of colors.
        /// </summary>
        /// <param name="size">Specifies the number	of components per color. Must be 3	or 4.</param>
        /// <param name="type">Specifies the data type of each color component in the array. Symbolic constants OpenGL.BYTE, OpenGL.UNSIGNED_BYTE, OpenGL.SHORT, OpenGL.UNSIGNED_SHORT, OpenGL.INT, OpenGL.UNSIGNED_INT, OpenGL.FLOAT and OpenGL.DOUBLE are accepted.</param>
        /// <param name="stride">Specifies the byte offset between consecutive colors. If stride is 0, (the initial value), the colors are understood to be tightly packed in the array.</param>
        /// <param name="pointer">Specifies a pointer to the first component of the first color element in the array.</param>
		 public static void ColorPointer (int size, uint type, int stride,  IntPtr pointer)
        {
            PreGLCall();
            glColorPointer(size, type, stride, pointer);
            PostGLCall();
        }

        /// <summary>
        /// Copy pixels in	the frame buffer.
        /// </summary>
        /// <param name="x">Specify the window coordinates of the lower left corner of the rectangular region of pixels to be copied.</param>
        /// <param name="y">Specify the window coordinates of the lower left corner of the rectangular region of pixels to be copied.</param>
        /// <param name="width">Specify the dimensions of the rectangular region of pixels to be copied. Both must be nonnegative.</param>
        /// <param name="height">Specify the dimensions of the rectangular region of pixels to be copied. Both must be nonnegative.</param>
        /// <param name="type">Specifies whether color values, depth values, or stencil values are to be copied. Symbolic constants OpenGL.COLOR, OpenGL.DEPTH, and OpenGL.STENCIL are accepted.</param>
		 public static void CopyPixels (int x, int y, int width, int height, uint type)
        {
            PreGLCall();
            glCopyPixels(x, y, width, height, type);
            PostGLCall();
        }

        /// <summary>
        /// Copy pixels into a 1D texture image.
        /// </summary>
        /// <param name="target">Specifies the target texture. Must be OpenGL.TEXTURE_1D.</param>
        /// <param name="level">Specifies the level-of-detail number. Level 0 is the base image level. Level n is the nth mipmap reduction image.</param>
        /// <param name="internalFormat">Specifies the internal format of the texture.</param>
        /// <param name="x">Specify the window coordinates of the left corner of the row of pixels to be copied.</param>
        /// <param name="y">Specify the window coordinates of the left corner of the row of pixels to be copied.</param>
        /// <param name="width">Specifies the width of the texture image. Must be 0 or 2^n = (2 * border) for some integer n. The height of the texture image is 1.</param>
        /// <param name="border">Specifies the width of the border. Must be either 0 or 1.</param>
		 public static void CopyTexImage1D (uint target, int level, uint internalFormat, int x, int y, int width, int border)
        {
            PreGLCall();
            glCopyTexImage1D(target, level, internalFormat, x, y, width, border);
            PostGLCall();
        }

        /// <summary>
        /// Copy pixels into a	2D texture image.
        /// </summary>
        /// <param name="target">Specifies the target texture. Must be OpenGL.TEXTURE_2D.</param>
        /// <param name="level">Specifies the level-of-detail number. Level 0 is the base image level. Level n is the nth mipmap reduction image.</param>
        /// <param name="internalFormat">Specifies the internal format of the texture.</param>
        /// <param name="x">Specify the window coordinates of the left corner of the row of pixels to be copied.</param>
        /// <param name="y">Specify the window coordinates of the left corner of the row of pixels to be copied.</param>
        /// <param name="width">Specifies the width of the texture image.</param>
        /// <param name="height">Specifies the height of the texture image.</param>
        /// <param name="border">Specifies the width of the border. Must be either 0 or 1.</param>
		 public static void CopyTexImage2D (uint target, int level, uint internalFormat, int x, int y, int width, int height, int border)
        {
            PreGLCall();
            glCopyTexImage2D(target, level, internalFormat, x, y, width, height, border);
            PostGLCall();
        }

        /// <summary>
        /// Copy a one-dimensional texture subimage.
        /// </summary>
        /// <param name="target">Specifies the target texture. Must be OpenGL.TEXTURE_1D.</param>
        /// <param name="level">Specifies the level-of-detail number. Level 0 is the base image level. Level n is the nth mipmap reduction image.</param>
        /// <param name="xoffset">Specifies the texel offset within the texture array.</param>
        /// <param name="x">Specify the window coordinates of the left corner of the row of pixels to be copied.</param>
        /// <param name="y">Specify the window coordinates of the left corner of the row of pixels to be copied.</param>
        /// <param name="width">Specifies the width of the texture image.</param>
		 public static void CopyTexSubImage1D (uint target, int level, int xoffset, int x, int y, int width)
        {
            PreGLCall();
            glCopyTexSubImage1D(target, level, xoffset, x, y, width);
            PostGLCall();
        }

        /// <summary>
        /// Copy a two-dimensional texture subimage.
        /// </summary>
        /// <param name="target">Specifies the target texture. Must be OpenGL.TEXTURE_2D.</param>
        /// <param name="level">Specifies the level-of-detail number. Level 0 is the base image level. Level n is the nth mipmap reduction image.</param>
        /// <param name="xoffset">Specifies the texel offset within the texture array.</param>
        /// <param name="yoffset">Specifies the texel offset within the texture array.</param>
        /// <param name="x">Specify the window coordinates of the left corner of the row of pixels to be copied.</param>
        /// <param name="y">Specify the window coordinates of the left corner of the row of pixels to be copied.</param>
        /// <param name="width">Specifies the width of the texture image.</param>
        /// <param name="height">Specifies the height of the texture image.</param>
		 public static void CopyTexSubImage2D (uint target, int level, int xoffset, int yoffset, int x, int y, int width, int height)
        {
            PreGLCall();
            glCopyTexSubImage2D(target, level, xoffset, yoffset, x, y, width, height);
            PostGLCall();
        }

        /// <summary>
        /// Specify whether front- or back-facing facets can be culled.
        /// </summary>
        /// <param name="mode">Specifies whether front- or back-facing facets are candidates for culling. Symbolic constants OpenGL.FRONT, OpenGL.BACK, and OpenGL.FRONT_AND_BACK are accepted. The initial	value is OpenGL.BACK.</param>
		 public static void CullFace (uint mode)
        {
            PreGLCall();
            glCullFace(mode);
            PostGLCall();
        }

		/// <summary>
		/// This function draws a sphere from the quadric object.
		/// </summary>
		/// <param name="qobj">The quadric object.</param>
		/// <param name="baseRadius">Radius at the base.</param>
		/// <param name="topRadius">Radius at the top.</param>
		/// <param name="height">Height of cylinder.</param>
		/// <param name="slices">Cylinder slices.</param>
		/// <param name="stacks">Cylinder stacks.</param>
		 public static void Cylinder(IntPtr qobj, double baseRadius, double topRadius, double height,int slices, int stacks)
		{
			PreGLCall();
			gluCylinder(qobj, baseRadius, topRadius, height, slices, stacks);
			PostGLCall();
		}

		/// <summary>
		/// This function deletes a list, or a range of lists.
		/// </summary>
		/// <param name="list">The list to delete.</param>
		/// <param name="range">The range of lists (often just 1).</param>
		 public static void DeleteLists(uint list, int range)
		{
			PreGLCall();
			glDeleteLists(list, range);
			PostGLCall();
		}

		/// <summary>
		/// This function deletes the underlying glu nurbs renderer.
		/// </summary>
		/// <param name="nurbsObject">The pointer to the nurbs object.</param>
		 public static void DeleteNurbsRenderer(IntPtr nurbsObject)
		{
			PreGLCall();
			gluDeleteNurbsRenderer(nurbsObject);
			PostGLCall();
        }

        /// <summary>
        /// This function deletes a set of Texture objects.
        /// </summary>
        /// <param name="n">Number of textures to delete.</param>
        /// <param name="textures">The array containing the names of the textures to delete.</param>
         public static void DeleteTextures(int n, uint[] textures)
        {
            PreGLCall();
            glDeleteTextures(n, textures);
            PostGLCall();
        }

        /// <summary>
        /// This function deletes a set of Texture objects.
        /// </summary>
        /// <param name="n">Number of textures to delete.</param>
        /// <param name="textures">The array containing the names of the textures to delete.</param>
         public static void DeleteTexture(uint texture)
        {
            PreGLCall();
            glDeleteTextures(1, new uint[] { texture });
            PostGLCall();
        }

        /// <summary>
        /// Call this function to delete an OpenGL Quadric object.
        /// </summary>
        /// <param name="quadric"></param>
         public static void DeleteQuadric(IntPtr quadric)
		{
            PreGLCall();
			gluDeleteQuadric(quadric);
            PostGLCall();
		}

		/// <summary>
		/// This function sets the current depth buffer comparison function, the default it LESS.
		/// </summary>
		/// <param name="func">The comparison function to set.</param>
		 public static void DepthFunc(uint func)
		{
			PreGLCall();
			glDepthFunc(func);
			PostGLCall();
		}
        
		/// <summary>
		/// This function sets the current depth buffer comparison function, the default it LESS.
		/// </summary>
        /// <param name="function">The comparison function to set.</param>
         public static void DepthFunc(DepthFunction function)
		{
			PreGLCall();
            glDepthFunc((uint)function);
			PostGLCall();
		}
        

		/// <summary>
		/// This function sets the depth mask.
		/// </summary>
		/// <param name="flag">The depth mask flag, normally 1.</param>
		 public static void DepthMask(byte flag)
		{
			PreGLCall();
			glDepthMask(flag);
			PostGLCall();
		}

        /// <summary>
        /// Specify mapping of depth values from normalized device coordinates	to window coordinates.
        /// </summary>
        /// <param name="zNear">Specifies the mapping of the near clipping plane to window coordinates. The initial value is 0.</param>
        /// <param name="zFar">Specifies the mapping of the near clipping plane to window coordinates. The initial value is 1.</param>
		 public static void DepthRange (double zNear, double zFar)
        {
            PreGLCall();
            glDepthRange(zNear, zFar);
            PostGLCall();
        }

		/// <summary>
		/// Call this function to disable an OpenGL capability.
		/// </summary>
		/// <param name="cap">The capability to disable.</param>
		 public static void Disable(uint cap)
		{
			PreGLCall();
			glDisable(cap);
			PostGLCall();
		}

		/// <summary>
		/// This function disables a client state array, such as a vertex array.
		/// </summary>
		/// <param name="array">The array to disable.</param>
		 public static void DisableClientState (uint array)
		{
			PreGLCall();
			glDisableClientState(array);
			PostGLCall();
		}

        /// <summary>
        /// Render	primitives from	array data.
        /// </summary>
        /// <param name="mode">Specifies what kind of primitives to render. Symbolic constants OpenGL.POINTS, OpenGL.LINE_STRIP, OpenGL.LINE_LOOP, OpenGL.LINES, OpenGL.TRIANGLE_STRIP, OpenGL.TRIANGLE_FAN, OpenGL.TRIANGLES, OpenGL.QUAD_STRIP, OpenGL.QUADS, and OpenGL.POLYGON are accepted.</param>
        /// <param name="first">Specifies the starting	index in the enabled arrays.</param>
        /// <param name="count">Specifies the number of indices to be rendered.</param>
		 public static void DrawArrays (uint mode, int first, int count)
        {
            PreGLCall();
            glDrawArrays(mode, first, count);
            PostGLCall();
        }

        /// <summary>
        /// Specify which color buffers are to be drawn into.
        /// </summary>
        /// <param name="mode">Specifies up to	four color buffers to be drawn into. Symbolic constants OpenGL.NONE, OpenGL.FRONT_LEFT, OpenGL.FRONT_RIGHT,	OpenGL.BACK_LEFT, OpenGL.BACK_RIGHT, OpenGL.FRONT, OpenGL.BACK, OpenGL.LEFT, OpenGL.RIGHT, OpenGL.FRONT_AND_BACK, and OpenGL.AUXi, where i is between 0 and (OpenGL.AUX_BUFFERS - 1), are accepted (OpenGL.AUX_BUFFERS is not the upper limit; use glGet to query the number of	available aux buffers.)  The initial value is OpenGL.FRONT for single- buffered contexts, and OpenGL.BACK for double-buffered contexts.</param>
		 public static void DrawBuffer (uint mode)
        {
            PreGLCall();
            glDrawBuffer(mode);
            PostGLCall();
        }

        /// <summary>
        /// Specify which color buffers are to be drawn into.
        /// </summary>
        /// <param name="drawBufferMode">Specifies up to	four color buffers to be drawn into.</param>
         public static void DrawBuffer(DrawBufferMode drawBufferMode)
        {
            PreGLCall();
            glDrawBuffer((uint)drawBufferMode);
            PostGLCall();
        }

        /// <summary>
        /// Render primitives from array data.
        /// </summary>
        /// <param name="mode">Specifies what kind of primitives to	render. Symbolic constants OpenGL.POINTS, OpenGL.LINE_STRIP, OpenGL.LINE_LOOP, OpenGL.LINES, OpenGL.TRIANGLE_STRIP, OpenGL.TRIANGLE_FAN, OpenGL.TRIANGLES, OpenGL.QUAD_STRIP, OpenGL.QUADS, and OpenGL.POLYGON are accepted.</param>
        /// <param name="count">Specifies the number of elements to be rendered.</param>
        /// <param name="indices">Specifies a pointer to the location where the indices are stored.</param>
         public static void DrawElements(uint mode, int count, uint[] indices)
        {
            PreGLCall();
            glDrawElements(mode, count, GL_UNSIGNED_INT, indices);
            PostGLCall();
        }

        /// <summary>
        /// Render primitives from array data.
        /// </summary>
        /// <param name="mode">Specifies what kind of primitives to	render. Symbolic constants OpenGL.POINTS, OpenGL.LINE_STRIP, OpenGL.LINE_LOOP, OpenGL.LINES, OpenGL.TRIANGLE_STRIP, OpenGL.TRIANGLE_FAN, OpenGL.TRIANGLES, OpenGL.QUAD_STRIP, OpenGL.QUADS, and OpenGL.POLYGON are accepted.</param>
        /// <param name="count">Specifies the number of elements to be rendered.</param>
        /// <param name="type">Specifies the type of the values in indices.	Must be one of OpenGL.UNSIGNED_BYTE, OpenGL.UNSIGNED_SHORT, or OpenGL.UNSIGNED_INT.</param>
        /// <param name="indices">Specifies a pointer to the location where the indices are stored.</param>
         public static void DrawElements(uint mode, int count, uint type, IntPtr indices)
        {
            PreGLCall();
            glDrawElements(mode, count, type, indices);
            PostGLCall();
        }

        /// <summary>
        /// Draws a rectangle of pixel data at the current raster position.
        /// </summary>
        /// <param name="width">Width of pixel data.</param>
        /// <param name="height">Height of pixel data.</param>
        /// <param name="format">Format of pixel data.</param>
        /// <param name="pixels">Pixel data buffer.</param>
         public static void DrawPixels(int width, int height, uint format, float[] pixels)
        {
          PreGLCall();
          glDrawPixels(width, height, format, GL_FLOAT, pixels);
          PostGLCall();
        }

        /// <summary>
        /// Draws a rectangle of pixel data at the current raster position.
        /// </summary>
        /// <param name="width">Width of pixel data.</param>
        /// <param name="height">Height of pixel data.</param>
        /// <param name="format">Format of pixel data.</param>
        /// <param name="pixels">Pixel data buffer.</param>
         public static void DrawPixels(int width, int height, uint format, uint[] pixels)
        {
          PreGLCall();
          glDrawPixels(width, height, format, GL_UNSIGNED_INT, pixels);
          PostGLCall();
        }

        /// <summary>
        /// Draws a rectangle of pixel data at the current raster position.
        /// </summary>
        /// <param name="width">Width of pixel data.</param>
        /// <param name="height">Height of pixel data.</param>
        /// <param name="format">Format of pixel data.</param>
        /// <param name="pixels">Pixel data buffer.</param>
         public static void DrawPixels(int width, int height, uint format, ushort[] pixels)
        {
          PreGLCall();
          glDrawPixels(width, height, format, GL_UNSIGNED_SHORT, pixels);
          PostGLCall();
        }

        /// <summary>
        /// Draws a rectangle of pixel data at the current raster position.
        /// </summary>
        /// <param name="width">Width of pixel data.</param>
        /// <param name="height">Height of pixel data.</param>
        /// <param name="format">Format of pixel data.</param>
        /// <param name="pixels">Pixel data buffer.</param>
         public static void DrawPixels(int width, int height, uint format, byte[] pixels)
        {
          PreGLCall();
          glDrawPixels(width, height, format, GL_UNSIGNED_BYTE, pixels);
          PostGLCall();
        }

        /// <summary>
        /// Draws a rectangle of pixel data at the current raster position.
        /// </summary>
        /// <param name="width">Width of pixel data.</param>
        /// <param name="height">Height of pixel data.</param>
        /// <param name="format">Format of pixel data.</param>
        /// <param name="type">The GL data type.</param>
        /// <param name="pixels">Pixel data buffer.</param>
         public static void DrawPixels(int width, int height, uint format, uint type, IntPtr pixels)
        {
          PreGLCall();
          glDrawPixels(width, height, format, type, pixels);
          PostGLCall();
        }

        /// <summary>
        /// Flag edges as either boundary or nonboundary.
        /// </summary>
        /// <param name="flag">Specifies the current edge flag	value, either OpenGL.TRUE or OpenGL.FALSE. The initial value is OpenGL.TRUE.</param>
		 public static void EdgeFlag (byte flag)
        {
            PreGLCall();
            glEdgeFlag(flag);
            PostGLCall();
        }

        /// <summary>
        /// Define an array of edge flags.
        /// </summary>
        /// <param name="stride">Specifies the byte offset between consecutive edge flags. If stride is	0 (the initial value), the edge	flags are understood to	be tightly packed in the array.</param>
        /// <param name="pointer">Specifies a pointer to the first edge flag in the array.</param>
		 public static void EdgeFlagPointer (int stride,  int[] pointer)
        {
            PreGLCall();
            glEdgeFlagPointer(stride, pointer);
            PostGLCall();
        }

        /// <summary>
        /// Flag edges as either boundary or nonboundary.
        /// </summary>
        /// <param name="flag">Specifies a pointer to an array that contains a single boolean element,	which replaces the current edge	flag value.</param>
		 public static void EdgeFlag( byte []flag)
        {
            PreGLCall();
            glEdgeFlagv(flag);
            PostGLCall();
        }

		/// <summary>
		/// Call this function to enable an OpenGL capability.
		/// </summary>
		/// <param name="cap">The capability you wish to enable.</param>
		 public static void Enable(uint cap)
		{
			PreGLCall();
			glEnable(cap);
			PostGLCall();
		}

		/// <summary>
		/// This function enables one of the client state arrays, such as a vertex array.
		/// </summary>
		/// <param name="array">The array to enable.</param>
		 public static void EnableClientState(uint array)
		{
			PreGLCall();
			glEnableClientState(array);
			PostGLCall();
		}

		/// <summary>
		/// This is not an imported OpenGL function, but very useful. If 'test' is
		/// true, cap is enabled, otherwise, it's disable.
		/// </summary>
		/// <param name="cap">The capability you want to enable.</param>
		/// <param name="test">The logical comparison.</param>
		 public static void EnableIf(uint cap, bool test)
		{
			if(test)	Enable(cap);
			else		Disable(cap);
		}

		/// <summary>
		/// Signals the End of drawing.
		/// </summary>
		 public static void End()
		{
			glEnd();
            
            //  Clear the begun flag.
            insideGLBegin = false;

            // This matches Begin()'s PreGLCall()
            PostGLCall();
		}

    /// <summary>
    /// This function ends the drawing of a NURBS curve.
    /// </summary>
    /// <param name="nurbsObject">The nurbs object.</param>
		 public static void EndCurve(IntPtr nurbsObject)
		{
			PreGLCall();
			gluEndCurve(nurbsObject);
			PostGLCall();
		}

		/// <summary>
		/// Ends the current display list compilation.
		/// </summary>
		 public static void EndList()
		{
			PreGLCall();
			glEndList();
			PostGLCall();
		}

    /// <summary>
    /// This function ends the drawing of a NURBS surface.
    /// </summary>
    /// <param name="nurbsObject">The nurbs object.</param>
		 public static void EndSurface(IntPtr nurbsObject)
		{
			PreGLCall();
			gluEndSurface(nurbsObject);
			PostGLCall();
		}
		
		/// <summary>
		/// Evaluate from the current evaluator.
		/// </summary>
		/// <param name="u">Domain coordinate.</param>
		 public static void EvalCoord1(double u)
		{
			PreGLCall();
			glEvalCoord1d(u);
			PostGLCall();
		}

        /// <summary>
        /// Evaluate from the current evaluator.
        /// </summary>
        /// <param name="u">Domain coordinate.</param>
		 public static void EvalCoord1( double []u)
        {
            PreGLCall();
            glEvalCoord1dv(u);
            PostGLCall();
        }

		/// <summary>
		/// Evaluate from the current evaluator.
		/// </summary>
		/// <param name="u">Domain coordinate.</param>
		 public static void EvalCoord1(float u)
		{
			PreGLCall();
			glEvalCoord1f(u);
			PostGLCall();
		}

        /// <summary>
        /// Evaluate from the current evaluator.
        /// </summary>
        /// <param name="u">Domain coordinate.</param>
		 public static void EvalCoord1( float []u)
        {
            PreGLCall();
            glEvalCoord1fv(u);
            PostGLCall();
        }

        /// <summary>
        /// Evaluate from the current evaluator.
        /// </summary>
        /// <param name="u">Domain coordinate.</param>
        /// <param name="v">Domain coordinate.</param>
		 public static void EvalCoord2(double u, double v)
        {
            PreGLCall();
            glEvalCoord2d(u, v);
            PostGLCall();
        }

        /// <summary>
        /// Evaluate from the current evaluator.
        /// </summary>
        /// <param name="u">Domain coordinate.</param>
         public static void EvalCoord2(double[] u)
        {
            PreGLCall();
            glEvalCoord2dv(u);
            PostGLCall();
        }

        /// <summary>
        /// Evaluate from the current evaluator.
        /// </summary>
        /// <param name="u">Domain coordinate.</param>
        /// <param name="v">Domain coordinate.</param>
         public static void EvalCoord2(float u, float v)
        {
            PreGLCall();
            glEvalCoord2f(u, v);
            PostGLCall();
        }

        /// <summary>
        /// Evaluate from the current evaluator.
        /// </summary>
        /// <param name="u">Domain coordinate.</param>
         public static void EvalCoord2(float[] u)
        {
            PreGLCall();
            glEvalCoord2fv(u);
            PostGLCall();
        }

		/// <summary>
		/// Evaluates a 'mesh' from the current evaluators.
		/// </summary>
		/// <param name="mode">Drawing mode, can be POINT or LINE.</param>
		/// <param name="i1">Beginning of range.</param>
		/// <param name="i2">End of range.</param>
		 public static void EvalMesh1(uint mode, int i1, int i2)
		{
			PreGLCall();
			glEvalMesh1(mode, i1, i2);
			PostGLCall();
		}
		/// <summary>
		/// Evaluates a 'mesh' from the current evaluators.
		/// </summary>
		/// <param name="mode">Drawing mode, fill, point or line.</param>
		/// <param name="i1">Beginning of range.</param>
		/// <param name="i2">End of range.</param>
		/// <param name="j1">Beginning of range.</param>
		/// <param name="j2">End of range.</param>
		 public static void EvalMesh2(uint mode, int i1, int i2, int j1, int j2)
		{
			PreGLCall();
			glEvalMesh2(mode, i1, i2, j1, j2);
			PostGLCall();
		}

        /// <summary>
        /// Generate and evaluate a single point in a mesh.
        /// </summary>
        /// <param name="i">The integer value for grid domain variable i.</param>
		 public static void EvalPoint1(int i)
        {
            PreGLCall();
            glEvalPoint1(i);
            PostGLCall();
        }

        /// <summary>
        /// Generate and evaluate a single point in a mesh.
        /// </summary>
        /// <param name="i">The integer value for grid domain variable i.</param>
        /// <param name="j">The integer value for grid domain variable j.</param>
		 public static void EvalPoint2(int i, int j)
        {
            PreGLCall();
            glEvalPoint2(i, j);
            PostGLCall();
        }

		/// <summary>
		/// This function sets the feedback buffer, that will receive feedback data.
		/// </summary>
		/// <param name="size">Size of the buffer.</param>
		/// <param name="type">Type of data in the buffer.</param>
		/// <param name="buffer">The buffer itself.</param>
		 public static void FeedbackBuffer(int size, uint type, float []buffer)
		{
			PreGLCall();
			glFeedbackBuffer(size, type, buffer);
			PostGLCall();
		}

		/// <summary>
		/// This function is similar to flush, but in a sense does it more, as it
		/// executes all commands aon both the client and the server.
		/// </summary>
		 public static void Finish()
		{
			PreGLCall();
			glFinish();
			PostGLCall();
		}

		/// <summary>
		/// This forces OpenGL to execute any commands you have given it.
		/// </summary>
		 public static void Flush()
		{
			PreGLCall();
			glFlush();
			PostGLCall();
		}

		/// <summary>
		/// Sets a fog parameter.
		/// </summary>
		/// <param name="pname">The parameter to set.</param>
		/// <param name="param">The value to set it to.</param>
		 public static void Fog(uint pname, float param)
		{
			PreGLCall();
			glFogf(pname, param);
			PostGLCall();
		}

		/// <summary>
		/// Sets a fog parameter.
		/// </summary>
		/// <param name="pname">The parameter to set.</param>
		/// <param name="parameters">The values to set it to.</param>
		 public static void Fog(uint pname,  float[] parameters)
		{
			PreGLCall();
			glFogfv(pname, parameters);
			PostGLCall();
		}

		/// <summary>
		/// Sets a fog parameter.
		/// </summary>
		/// <param name="pname">The parameter to set.</param>
		/// <param name="param">The value to set it to.</param>
		 public static void Fog(uint pname, int param)
		{
			PreGLCall();
			glFogi(pname, param);
			PostGLCall();
		}

		/// <summary>
		/// Sets a fog parameter.
		/// </summary>
		/// <param name="pname">The parameter to set.</param>
		/// <param name="parameters">The values to set it to.</param>
		 public static void Fog(uint pname,  int[] parameters)
		{
			PreGLCall();
			glFogiv(pname, parameters);
			PostGLCall();
		}

		/// <summary>
		/// This function sets what defines a front face.
		/// </summary>
		/// <param name="mode">Winding mode, counter clockwise by default.</param>
		 public static void FrontFace(uint mode)
		{
			PreGLCall();
			glFrontFace(mode);
			PostGLCall();
		}

		/// <summary>
		/// This function creates a frustrum transformation and mulitplies it to the current
		/// matrix (which in most cases should be the projection matrix).
		/// </summary>
		/// <param name="left">Left clip position.</param>
		/// <param name="right">Right clip position.</param>
		/// <param name="bottom">Bottom clip position.</param>
		/// <param name="top">Top clip position.</param>
		/// <param name="zNear">Near clip position.</param>
		/// <param name="zFar">Far clip position.</param>
		 public static void Frustum(double left, double right, double bottom, 
			double top, double zNear, double zFar)
		{
			PreGLCall();
			glFrustum(left, right, bottom, top, zNear, zFar);
			PostGLCall();
		}

		/// <summary>
		/// This function generates 'range' number of contiguos display list indices.
		/// </summary>
		/// <param name="range">The number of lists to generate.</param>
		/// <returns>The first list.</returns>
		public static uint GenLists(int range)
		{
			PreGLCall();
			uint list = glGenLists(range);
			PostGLCall();

			return list;
        }

        /// <summary>
        /// Create a set of unique texture names.
        /// </summary>
        /// <param name="n">Number of names to create.</param>
        /// <param name="textures">Array to store the texture names.</param>
         public static void GenTextures(int n, uint[] textures)
        {
            PreGLCall();
            glGenTextures(n, textures);
            PostGLCall();
        }

        /// <summary>
        /// Create a set of unique texture names.
        /// </summary>
        /// <param name="n">Number of names to create.</param>
        public static uint[] GenTextures(int n)
        {
            PreGLCall();
            uint[] textures = new uint[n];
            glGenTextures(n, textures);
            PostGLCall();
            return textures;
        }

        /// <summary>
        /// Create a set of unique texture names.
        /// </summary>
        /// <param name="n">Number of names to create.</param>
        public static uint GenTexture()
        {
            PreGLCall();
            uint[] textures = new uint[1];
            glGenTextures(1, textures);
            PostGLCall();
            return textures[0];
        }

        /// <summary>
        /// This function queries OpenGL for data, and puts it in the buffer supplied.
        /// </summary>
        /// <param name="pname">The parameter to query.</param>
        /// <param name="parameters"></param>
         public static void GetBooleanv (uint pname, byte[] parameters)
		{
			PreGLCall();
			glGetBooleanv(pname, parameters);
			PostGLCall();
		}

        /// <summary>
        /// This function queries OpenGL for data, and puts it in the buffer supplied.
        /// </summary>
        /// <param name="pname">The parameter to query.</param>
        /// <param name="parameters"></param>
         public static void GetBooleanv(GetTarget pname, byte[] parameters)
        {
            PreGLCall();
            glGetBooleanv((uint)pname, parameters);
            PostGLCall();
        }

        /// <summary>
        /// Return the coefficients of the specified clipping plane.
        /// </summary>
        /// <param name="plane">Specifies a	clipping plane.	 The number of clipping planes depends on the implementation, but at least six clipping planes are supported. They are identified by symbolic names of the form OpenGL.CLIP_PLANEi where 0 Less Than i Less Than OpenGL.MAX_CLIP_PLANES.</param>
        /// <param name="equation">Returns four double-precision values that are the coefficients of the plane equation of plane in eye coordinates. The initial value is (0, 0, 0, 0).</param>
		 public static void GetClipPlane (uint plane, double []equation)
        {
            PreGLCall();
            glGetClipPlane(plane, equation);
            PostGLCall();
        }

		/// <summary>
		/// This function queries OpenGL for data, and puts it in the buffer supplied.
		/// </summary>
		/// <param name="pname">The parameter to query.</param>
		/// <param name="parameters">The buffer to put that data into.</param>
		 public static void GetDouble(uint pname, double []parameters)
		{
			PreGLCall();
			glGetDoublev(pname, parameters);
			PostGLCall();
		}

        /// <summary>
        /// This function queries OpenGL for data, and puts it in the buffer supplied.
        /// </summary>
        /// <param name="pname">The parameter to query.</param>
        /// <param name="parameters">The buffer to put that data into.</param>
         public static void GetDouble(GetTarget pname, double[] parameters)
        {
            PreGLCall();
            glGetDoublev((uint)pname, parameters);
            PostGLCall();
        }

		/// <summary>
		/// Get the current OpenGL error code.
		/// </summary>
		/// <returns>The current OpenGL error code.</returns>
		public static uint GetError()
		{
			return glGetError();
		}

        /// <summary>
        /// Get the current OpenGL error code.
        /// </summary>
        /// <returns>The current OpenGL error code.</returns>
        public static ErrorCode GetErrorCode()
        {
            return (ErrorCode)glGetError();
        }

		/// <summary>
		/// This this function to query OpenGL values.
		/// </summary>
		/// <param name="pname">The parameter to query.</param>
		/// <param name="parameters">The parameters</param>
		 public static void GetFloat(uint pname, float[] parameters)
		{
			PreGLCall();
			glGetFloatv(pname, parameters);
			PostGLCall();
		}

        /// <summary>
        /// This this function to query OpenGL values.
        /// </summary>
        /// <param name="pname">The parameter to query.</param>
        /// <param name="parameters">The parameters</param>
         public static void GetFloat(GetTarget pname, float[] parameters)
        {
            PreGLCall();
            glGetFloatv((uint)pname, parameters);
            PostGLCall();
        }

		/// <summary>
		/// Use this function to query OpenGL parameter values.
		/// </summary>
		/// <param name="pname">The Parameter to query</param>
		/// <param name="parameters">An array to put the values into.</param>
		 public static void GetInteger(uint pname, int[] parameters)
		{
			PreGLCall();
			glGetIntegerv(pname, parameters);
			PostGLCall();
		}

        /// <summary>
        /// Use this function to query OpenGL parameter values.
        /// </summary>
        /// <param name="pname">The Parameter to query</param>
        /// <param name="parameters">An array to put the values into.</param>
         public static void GetInteger(GetTarget pname, int[] parameters)
        {
            PreGLCall();
            glGetIntegerv((uint)pname, parameters);
            PostGLCall();
        }

        /// <summary>
        /// Return light source parameter values.
        /// </summary>
        /// <param name="light">Specifies a light source. The number of possible lights depends on the implementation, but at least eight lights are supported. They are identified by symbolic names of the form OpenGL.LIGHTi where i ranges from 0 to the value of OpenGL.GL_MAX_LIGHTS - 1.</param>
        /// <param name="pname">Specifies a light source parameter for light.</param>
        /// <param name="parameters">Returns the requested data.</param>
		 public static void GetLight(uint light, uint pname, float []parameters)
        {
            PreGLCall();
            glGetLightfv(light, pname, parameters);
            PostGLCall();
        }

        /// <summary>
        /// Return light source parameter values.
        /// </summary>
        /// <param name="light">Specifies a light source. The number of possible lights depends on the implementation, but at least eight lights are supported. They are identified by symbolic names of the form OpenGL.LIGHTi where i ranges from 0 to the value of OpenGL.GL_MAX_LIGHTS - 1.</param>
        /// <param name="pname">Specifies a light source parameter for light.</param>
        /// <param name="parameters">Returns the requested data.</param>
         public static void GetLight(uint light, uint pname, int[] parameters)
        {
            PreGLCall();
            glGetLightiv(light, pname, parameters);
            PostGLCall();
        }

        /// <summary>
        /// Return evaluator parameters.
        /// </summary>
        /// <param name="target">Specifies the symbolic name of a map.</param>
        /// <param name="query">Specifies which parameter to return.</param>
        /// <param name="v">Returns the requested data.</param>
		 public static void GetMap(uint target, uint query, double []v)
        {
            PreGLCall();
            glGetMapdv(target, query, v);
            PostGLCall();
        }

        /// <summary>
        /// Return evaluator parameters.
        /// </summary>
        /// <param name="target">Specifies the symbolic name of a map.</param>
        /// <param name="query">Specifies which parameter to return.</param>
        /// <param name="v">Returns the requested data.</param>
         public static void GetMap(GetMapTarget target, uint query, double[] v)
        {
            PreGLCall();
            glGetMapdv((uint)target, query, v);
            PostGLCall();
        }

        /// <summary>
        /// Return evaluator parameters.
        /// </summary>
        /// <param name="target">Specifies the symbolic name of a map.</param>
        /// <param name="query">Specifies which parameter to return.</param>
        /// <param name="v">Returns the requested data.</param>
         public static void GetMap(GetMapTarget target, uint query, float[] v)
        {
            PreGLCall();
            glGetMapfv((uint)target, query, v);
            PostGLCall();
        }

        /// <summary>
        /// Return evaluator parameters.
        /// </summary>
        /// <param name="target">Specifies the symbolic name of a map.</param>
        /// <param name="query">Specifies which parameter to return.</param>
        /// <param name="v">Returns the requested data.</param>
		 public static void GetMap(uint target, uint query, float []v)
        {
            PreGLCall();
            glGetMapfv(target, query, v);
            PostGLCall();
        }

        /// <summary>
        /// Return evaluator parameters.
        /// </summary>
        /// <param name="target">Specifies the symbolic name of a map.</param>
        /// <param name="query">Specifies which parameter to return.</param>
        /// <param name="v">Returns the requested data.</param>
         public static void GetMap(GetMapTarget target, uint query, int[] v)
        {
            PreGLCall();
            glGetMapiv((uint)target, query, v);
            PostGLCall();
        }

        /// <summary>
        /// Return evaluator parameters.
        /// </summary>
        /// <param name="target">Specifies the symbolic name of a map.</param>
        /// <param name="query">Specifies which parameter to return.</param>
        /// <param name="v">Returns the requested data.</param>
		 public static void GetMap(uint target, uint query, int []v)
        {
            PreGLCall();
            glGetMapiv(target, query, v);
            PostGLCall();
        }

        /// <summary>
        /// Return material parameters.
        /// </summary>
        /// <param name="face">Specifies which of the two materials is being queried. OpenGL.FRONT or OpenGL.BACK are accepted, representing the front and back materials, respectively.</param>
        /// <param name="pname">Specifies the material parameter to return.</param>
        /// <param name="parameters">Returns the requested data.</param>
         public static void GetMaterial(uint face, uint pname, float[] parameters)
        {
            PreGLCall();
            glGetMaterialfv(face, pname, parameters);
            PostGLCall();
        }

        /// <summary>
        /// Return material parameters.
        /// </summary>
        /// <param name="face">Specifies which of the two materials is being queried. OpenGL.FRONT or OpenGL.BACK are accepted, representing the front and back materials, respectively.</param>
        /// <param name="pname">Specifies the material parameter to return.</param>
        /// <param name="parameters">Returns the requested data.</param>
         public static void GetMaterial(uint face, uint pname, int[] parameters)
        {
            PreGLCall();
            glGetMaterialiv(face, pname, parameters);
            PostGLCall();
        }

        /// <summary>
        /// Return the specified pixel map.
        /// </summary>
        /// <param name="map">Specifies the	name of	the pixel map to return.</param>
        /// <param name="values">Returns the pixel map	contents.</param>
		 public static void GetPixelMap(uint map, float []values)
        {
            PreGLCall();
            glGetPixelMapfv(map, values);
            PostGLCall();
        }

        /// <summary>
        /// Return the specified pixel map.
        /// </summary>
        /// <param name="map">Specifies the	name of	the pixel map to return.</param>
        /// <param name="values">Returns the pixel map	contents.</param>
		 public static void GetPixelMap(uint map, uint []values)
        {
            PreGLCall();
            glGetPixelMapuiv(map, values);
            PostGLCall();
        }

        /// <summary>
        /// Return the specified pixel map.
        /// </summary>
        /// <param name="map">Specifies the	name of	the pixel map to return.</param>
        /// <param name="values">Returns the pixel map	contents.</param>
		 public static void GetPixelMap(uint map, ushort []values)
        {
            PreGLCall();
            glGetPixelMapusv(map, values);
            PostGLCall();
        }

        /// <summary>
        /// Return the address of the specified pointer.
        /// </summary>
        /// <param name="pname">Specifies the array or buffer pointer to be returned.</param>
        /// <param name="parameters">Returns the pointer value specified by parameters.</param>
		 public static void GetPointerv (uint pname, int[] parameters)
        {
            PreGLCall();
            glGetPointerv(pname, parameters);
            PostGLCall();
        }

        /// <summary>
        /// Return the polygon stipple pattern.
        /// </summary>
        /// <param name="mask">Returns the stipple pattern. The initial value is all 1's.</param>
		 public static void GetPolygonStipple (byte []mask)
        {
            PreGLCall();
            glGetPolygonStipple(mask);
            PostGLCall();
        }

        /// <summary>
        /// Return a string	describing the current GL connection.
        /// </summary>
        /// <param name="name">Specifies a symbolic constant, one of OpenGL.VENDOR, OpenGL.RENDERER, OpenGL.VERSION, or OpenGL.EXTENSIONS.</param>
        /// <returns>Pointer to the specified string.</returns>
		public static unsafe string GetString (uint name)
		{
            PreGLCall();
			sbyte* pStr = glGetString(name);
			var str = new string(pStr);
            PostGLCall();

            return str;
		}

        /// <summary>
        /// Return texture environment parameters.
        /// </summary>
        /// <param name="target">Specifies a texture environment.  Must be OpenGL.TEXTURE_ENV.</param>
        /// <param name="pname">Specifies the	symbolic name of a texture environment parameter.  Accepted values are OpenGL.TEXTURE_ENV_MODE, and OpenGL.TEXTURE_ENV_COLOR.</param>
        /// <param name="parameters">Returns the requested	data.</param>
		 public static void GetTexEnv(uint target, uint pname, float []parameters)
        {
            PreGLCall();
            glGetTexEnvfv(target, pname, parameters);
            PostGLCall();
        }

        /// <summary>
        /// Return texture environment parameters.
        /// </summary>
        /// <param name="target">Specifies a texture environment.  Must be OpenGL.TEXTURE_ENV.</param>
        /// <param name="pname">Specifies the	symbolic name of a texture environment parameter.  Accepted values are OpenGL.TEXTURE_ENV_MODE, and OpenGL.TEXTURE_ENV_COLOR.</param>
        /// <param name="parameters">Returns the requested	data.</param>
         public static void GetTexEnv(uint target, uint pname, int[] parameters)
        {
            PreGLCall();
            glGetTexEnviv(target, pname, parameters);
            PostGLCall();
        }

        /// <summary>
        /// Control the generation of texture coordinates.
        /// </summary>
        /// <param name="coord">Specifies a texture coordinate. Must be one of OpenGL.S, OpenGL.T, OpenGL.R, or OpenGL.Q.</param>
        /// <param name="pname">Specifies the symbolic name of the texture-coordinate generation function. Must be OpenGL.TEXTURE_GEN_MODE.</param>
        /// <param name="parameters">Specifies a single-valued texture generation parameter, one of OpenGL.OBJECT_LINEAR, OpenGL.EYE_LINEAR, or OpenGL.SPHERE_MAP.</param>
         public static void GetTexGen(uint coord, uint pname, double[] parameters) 
        {
            PreGLCall();
            glGetTexGendv(coord, pname, parameters);
            PostGLCall();
        }

        /// <summary>
        /// Control the generation of texture coordinates.
        /// </summary>
        /// <param name="coord">Specifies a texture coordinate. Must be one of OpenGL.S, OpenGL.T, OpenGL.R, or OpenGL.Q.</param>
        /// <param name="pname">Specifies the symbolic name of the texture-coordinate generation function. Must be OpenGL.TEXTURE_GEN_MODE.</param>
        /// <param name="parameters">Specifies a single-valued texture generation parameter, one of OpenGL.OBJECT_LINEAR, OpenGL.EYE_LINEAR, or OpenGL.SPHERE_MAP.</param>
         public static void GetTexGen(uint coord, uint pname, float[] parameters)
        {
            PreGLCall();
            glGetTexGenfv(coord, pname, parameters);
            PostGLCall();
        }
        
        /// <summary>
        /// Control the generation of texture coordinates.
        /// </summary>
        /// <param name="coord">Specifies a texture coordinate. Must be one of OpenGL.S, OpenGL.T, OpenGL.R, or OpenGL.Q.</param>
        /// <param name="pname">Specifies the symbolic name of the texture-coordinate generation function. Must be OpenGL.TEXTURE_GEN_MODE.</param>
        /// <param name="parameters">Specifies a single-valued texture generation parameter, one of OpenGL.OBJECT_LINEAR, OpenGL.EYE_LINEAR, or OpenGL.SPHERE_MAP.</param>
         public static void GetTexGen(uint coord, uint pname, int[] parameters)
        {
            PreGLCall();
            glGetTexGeniv(coord, pname, parameters);
            PostGLCall();
        }

        /// <summary>
        /// Return a texture image.
        /// </summary>
        /// <param name="target">Specifies which texture is to	be obtained. OpenGL.TEXTURE_1D and OpenGL.TEXTURE_2D are accepted.</param>
        /// <param name="level">Specifies the level-of-detail number of the desired image.  Level	0 is the base image level.  Level n is the nth mipmap reduction image.</param>
        /// <param name="format">Specifies a pixel format for the returned data.</param>
        /// <param name="type">Specifies a pixel type for the returned data.</param>
        /// <param name="pixels">Returns the texture image.  Should be	a pointer to an array of the type specified by type.</param>
		 public static void GetTexImage (uint target, int level, uint format, uint type, int []pixels)
        {
            PreGLCall();
            glGetTexImage(target, level, format, type, pixels);
            PostGLCall();
        }

        /// <summary>
        /// Return texture parameter values for a specific level of detail.
        /// </summary>
        /// <param name="target">Specifies the	symbolic name of the target texture.</param>
        /// <param name="level">Specifies the level-of-detail	number of the desired image.  Level	0 is the base image level.  Level n is the nth mipmap reduction image.</param>
        /// <param name="pname">Specifies the symbolic name of a texture parameter.</param>
        /// <param name="parameters">Returns the requested	data.</param>
		 public static void GetTexLevelParameter(uint target, int level, uint pname, float []parameters)
        {
            PreGLCall();
            glGetTexLevelParameterfv(target, level, pname, parameters);
            PostGLCall();
        }

        /// <summary>
        /// Return texture parameter values for a specific level of detail.
        /// </summary>
        /// <param name="target">Specifies the	symbolic name of the target texture.</param>
        /// <param name="level">Specifies the level-of-detail	number of the desired image.  Level	0 is the base image level.  Level n is the nth mipmap reduction image.</param>
        /// <param name="pname">Specifies the symbolic name of a texture parameter.</param>
        /// <param name="parameters">Returns the requested	data.</param>
         public static void GetTexLevelParameter(uint target, int level, uint pname, int[] parameters)
        {
            PreGLCall();
            glGetTexLevelParameteriv(target, level, pname, parameters);
            PostGLCall();
        }

        /// <summary>
        /// Return texture parameter values.
        /// </summary>
        /// <param name="target">Specifies the symbolic name of the target texture.</param>
        /// <param name="pname">Specifies the symbolic name of a texture parameter.</param>
        /// <param name="parameters">Returns the texture parameters.</param>
         public static void GetTexParameter(uint target, uint pname, float[] parameters) 
        {
            PreGLCall();
            glGetTexParameterfv(target, pname, parameters);
            PostGLCall();
        }
        /// <summary>
        /// Return texture parameter values.
        /// </summary>
        /// <param name="target">Specifies the symbolic name of the target texture.</param>
        /// <param name="pname">Specifies the symbolic name of a texture parameter.</param>
        /// <param name="parameters">Returns the texture parameters.</param>
         public static void GetTexParameter(uint target, uint pname, int[] parameters)
        {
            PreGLCall();
            glGetTexParameteriv(target, pname, parameters);
            PostGLCall();
        }

        /// <summary>
        /// Specify implementation-specific hints.
        /// </summary>
        /// <param name="target">Specifies a symbolic constant indicating the behavior to be controlled.</param>
        /// <param name="mode">Specifies a symbolic constant indicating the desired behavior.</param>
		 public static void Hint (uint target, uint mode)
        {
            PreGLCall();
            glHint(target, mode);
            PostGLCall();
        }

        /// <summary>
        /// Specify implementation-specific hints.
        /// </summary>
        /// <param name="target">Specifies a symbolic constant indicating the behavior to be controlled.</param>
        /// <param name="mode">Specifies a symbolic constant indicating the desired behavior.</param>
         public static void Hint(HintTarget target, HintMode mode)
        {
            PreGLCall();
            glHint((uint)target, (uint)mode);
            PostGLCall();
        }

        /// <summary>
        /// Control	the writing of individual bits in the color	index buffers.
        /// </summary>
        /// <param name="mask">Specifies a bit	mask to	enable and disable the writing of individual bits in the color index buffers. Initially, the mask is all 1's.</param>
		 public static void IndexMask (uint mask)
        {
            PreGLCall();
            glIndexMask(mask);
            PostGLCall();
        }

        /// <summary>
        /// Define an array of color indexes.
        /// </summary>
        /// <param name="type">Specifies the data type of each color index in the array.  Symbolic constants OpenGL.UNSIGNED_BYTE, OpenGL.SHORT, OpenGL.INT, OpenGL.FLOAT, and OpenGL.DOUBLE are accepted.</param>
        /// <param name="stride">Specifies the byte offset between consecutive color indexes.  If stride is 0 (the initial value), the color indexes are understood	to be tightly packed in the array.</param>
        /// <param name="pointer">Specifies a pointer to the first index in the array.</param>
		 public static void IndexPointer (uint type, int stride,  int[] pointer)
        {
            PreGLCall();
            glIndexPointer(type, stride, pointer);
            PostGLCall();
        }

        /// <summary>
        /// Set the current color index.
        /// </summary>
        /// <param name="c">Specifies the new value for the current color index.</param>
		 public static void Index(double c)
        {
            PreGLCall();
            glIndexd(c);
            PostGLCall();
        }

        /// <summary>
        /// Set the current color index.
        /// </summary>
        /// <param name="c">Specifies the new value for the current color index.</param>
         public static void Index(double[] c)
        {
            PreGLCall();
            glIndexdv(c);
            PostGLCall();
        }

        /// <summary>
        /// Set the current color index.
        /// </summary>
        /// <param name="c">Specifies the new value for the current color index.</param>
         public static void Index(float c)
        {
            PreGLCall();
            glIndexf(c);
            PostGLCall();
        }

        /// <summary>
        /// Set the current color index.
        /// </summary>
        /// <param name="c">Specifies the new value for the current color index.</param>
         public static void Index(float[] c)
        {
            PreGLCall();
            glIndexfv(c);
            PostGLCall();
        }

        /// <summary>
        /// Set the current color index.
        /// </summary>
        /// <param name="c">Specifies the new value for the current color index.</param>
         public static void Index(int c)
        {
            PreGLCall();
            glIndexi(c);
            PostGLCall();
        }

        /// <summary>
        /// Set the current color index.
        /// </summary>
        /// <param name="c">Specifies the new value for the current color index.</param>
         public static void Index(int[] c)
        {
            PreGLCall();
            glIndexiv(c);
            PostGLCall();
        }

        /// <summary>
        /// Set the current color index.
        /// </summary>
        /// <param name="c">Specifies the new value for the current color index.</param>
         public static void Index(short c)
        {
            PreGLCall();
            glIndexs(c);
            PostGLCall();
        }

        /// <summary>
        /// Set the current color index.
        /// </summary>
        /// <param name="c">Specifies the new value for the current color index.</param>
         public static void Index(short[] c)
        {
            PreGLCall();
            glIndexsv(c);
            PostGLCall();
        }

        /// <summary>
        /// Set the current color index.
        /// </summary>
        /// <param name="c">Specifies the new value for the current color index.</param>
         public static void Index(byte c)
        {
            PreGLCall();
            glIndexub(c);
            PostGLCall();
        }

        /// <summary>
        /// Set the current color index.
        /// </summary>
        /// <param name="c">Specifies the new value for the current color index.</param>
         public static void Index(byte[] c)
        {
            PreGLCall();
            glIndexubv(c);
            PostGLCall();
        }

		/// <summary>
		/// This function initialises the select buffer names.
		/// </summary>
		 public static void InitNames()
		{
			PreGLCall();
			glInitNames();
			PostGLCall();
		}

        /// <summary>
        /// Simultaneously specify and enable several interleaved arrays.
        /// </summary>
        /// <param name="format">Specifies the type of array to enable.</param>
        /// <param name="stride">Specifies the offset in bytes between each aggregate array element.</param>
        /// <param name="pointer">The array.</param>
		 public static void InterleavedArrays (uint format, int stride,  int[] pointer)
        {
            PreGLCall();
            glInterleavedArrays(format, stride, pointer);
            PostGLCall();
        }

		/// <summary>
		/// Use this function to query if a certain OpenGL function is enabled or not.
		/// </summary>
		/// <param name="cap">The capability to test.</param>
		/// <returns>True if the capability is enabled, otherwise, false.</returns>
		public static bool IsEnabled (uint cap)
		{
			PreGLCall();
			byte e = glIsEnabled(cap);
			PostGLCall();

			return e != 0;
		}

		/// <summary>
		/// This function determines whether a specified value is a display list.
		/// </summary>
		/// <param name="list">The value to test.</param>
		/// <returns>TRUE if it is a list, FALSE otherwise.</returns>
		public static byte IsList(uint list)
		{
			PreGLCall();
			byte islist = glIsList(list);
			PostGLCall();

			return islist;
		}

        /// <summary>
        /// Determine if a name corresponds	to a texture.
        /// </summary>
        /// <param name="texture">Specifies a value that may be the name of a texture.</param>
        /// <returns>True if texture is a texture object.</returns>
		public static byte IsTexture (uint texture)
        {
            PreGLCall();
            byte returnValue = glIsTexture(texture);
            PostGLCall();

            return returnValue;
        }

		/// <summary>
		/// This function sets a parameter of the lighting model.
		/// </summary>
		/// <param name="pname">The name of the parameter.</param>
		/// <param name="param">The parameter to set it to.</param>
		 public static void LightModel(uint pname, float param)
		{
			PreGLCall();
			glLightModelf(pname, param);
			PostGLCall();
		}

		/// <summary>
		/// This function sets a parameter of the lighting model.
		/// </summary>
		/// <param name="pname">The name of the parameter.</param>
		/// <param name="param">The parameter to set it to.</param>
         public static void LightModel(LightModelParameter pname, float param)
		{
			PreGLCall();
			glLightModelf((uint)pname, param);
			PostGLCall();
		}

		/// <summary>
		/// This function sets a parameter of the lighting model.
		/// </summary>
		/// <param name="pname">The name of the parameter.</param>
		/// <param name="parameters">The parameter to set it to.</param>
		 public static void LightModel(uint pname,  float[] parameters)
		{
			PreGLCall();
			glLightModelfv(pname, parameters);
			PostGLCall();
		}

		/// <summary>
		/// This function sets a parameter of the lighting model.
		/// </summary>
		/// <param name="pname">The name of the parameter.</param>
		/// <param name="parameters">The parameter to set it to.</param>
         public static void LightModel(LightModelParameter pname, float[] parameters)
		{
			PreGLCall();
			glLightModelfv((uint)pname, parameters);
			PostGLCall();
		}

		/// <summary>
		/// This function sets a parameter of the lighting model.
		/// </summary>
		/// <param name="pname">The name of the parameter.</param>
		/// <param name="param">The parameter to set it to.</param>
		 public static void LightModel(uint pname, int param)
		{
			PreGLCall();
			glLightModeli(pname, param);
			PostGLCall();
		}

		/// <summary>
		/// This function sets a parameter of the lighting model.
		/// </summary>
		/// <param name="pname">The name of the parameter.</param>
		/// <param name="param">The parameter to set it to.</param>
         public static void LightModel(LightModelParameter pname, int param)
		{
			PreGLCall();
			glLightModeli((uint)pname, param);
			PostGLCall();
		}

		/// <summary>
		/// This function sets a parameter of the lighting model.
		/// </summary>
		/// <param name="pname">The name of the parameter.</param>
		/// <param name="parameters">The parameter to set it to.</param>
		 public static void LightModel (uint pname, int[] parameters)
		{
			PreGLCall();
			glLightModeliv(pname, parameters);
			PostGLCall();
		}

		/// <summary>
		/// This function sets a parameter of the lighting model.
		/// </summary>
		/// <param name="pname">The name of the parameter.</param>
		/// <param name="parameters">The parameter to set it to.</param>
         public static void LightModel(LightModelParameter pname, int[] parameters)
		{
			PreGLCall();
			glLightModeliv((uint)pname, parameters);
			PostGLCall();
		}

		/// <summary>
		/// Set the parameter (pname) of the light 'light'.
		/// </summary>
		/// <param name="light">The light you wish to set parameters for.</param>
		/// <param name="pname">The parameter you want to set.</param>
		/// <param name="param">The value that you want to set the parameter to.</param>
		 public static void Light(uint light, uint pname, float param)
		{
            PreGLCall();
			glLightf(light, pname, param);
			PostGLCall();
		}

        /// <summary>
        /// Set the parameter (pname) of the light 'light'.
        /// </summary>
        /// <param name="light">The light you wish to set parameters for.</param>
        /// <param name="pname">The parameter you want to set.</param>
        /// <param name="param">The value that you want to set the parameter to.</param>
         public static void Light(LightName light, LightParameter pname, float param)
        {
            PreGLCall();
            glLightf((uint)light, (uint)pname, param);
            PostGLCall();
        }

		/// <summary>
		/// Set the parameter (pname) of the light 'light'.
		/// </summary>
		/// <param name="light">The light you wish to set parameters for.</param>
		/// <param name="pname">The parameter you want to set.</param>
		/// <param name="parameters">The value that you want to set the parameter to.</param>
		 public static void Light(uint light, uint pname,  float[] parameters)
		{
			PreGLCall();
			glLightfv(light, pname, parameters);
			PostGLCall();
		}
        
        /// <summary>
        /// Set the parameter (pname) of the light 'light'.
        /// </summary>
        /// <param name="light">The light you wish to set parameters for.</param>
        /// <param name="pname">The parameter you want to set.</param>
        /// <param name="parameters">The value that you want to set the parameter to.</param>
         public static void Light(LightName light, LightParameter pname, float[] parameters)
        {
            PreGLCall();
            glLightfv((uint)light, (uint)pname, parameters);
            PostGLCall();
        }

		/// <summary>
		/// Set the parameter (pname) of the light 'light'.
		/// </summary>
		/// <param name="light">The light you wish to set parameters for.</param>
		/// <param name="pname">The parameter you want to set.</param>
		/// <param name="param">The value that you want to set the parameter to.</param>
		 public static void Light(uint light, uint pname, int param)
		{
			PreGLCall();
			glLighti(light, pname, param);
			PostGLCall();
		}

        /// <summary>
        /// Set the parameter (pname) of the light 'light'.
        /// </summary>
        /// <param name="light">The light you wish to set parameters for.</param>
        /// <param name="pname">The parameter you want to set.</param>
        /// <param name="param">The value that you want to set the parameter to.</param>
         public static void Light(LightName light, LightParameter pname, int param)
        {
            PreGLCall();
            glLighti((uint)light, (uint)pname, param);
            PostGLCall();
        }

        /// <summary>
        /// Set the parameter (pname) of the light 'light'.
        /// </summary>
        /// <param name="light">The light you wish to set parameters for.</param>
        /// <param name="pname">The parameter you want to set.</param>
        /// <param name="parameters">The parameters.</param>
		 public static void Light(uint light, uint pname,  int []parameters)
		{
			PreGLCall();
			glLightiv(light, pname, parameters);
			PostGLCall();
		}

        /// <summary>
        /// Set the parameter (pname) of the light 'light'.
        /// </summary>
        /// <param name="light">The light you wish to set parameters for.</param>
        /// <param name="pname">The parameter you want to set.</param>
        /// <param name="parameters">The parameters.</param>
         public static void Light(LightName light, LightParameter pname, int[] parameters)
        {
            PreGLCall();
            glLightiv((uint)light, (uint)pname, parameters);
            PostGLCall();
        }

        /// <summary>
        /// Specify the line stipple pattern.
        /// </summary>
        /// <param name="factor">Specifies a multiplier for each bit in the line stipple pattern.  If factor is 3, for example, each bit in the pattern is used three times before the next	bit in the pattern is used. factor is clamped to the range	[1, 256] and defaults to 1.</param>
        /// <param name="pattern">Specifies a 16-bit integer whose bit	pattern determines which fragments of a line will be drawn when	the line is rasterized.	 Bit zero is used first; the default pattern is all 1's.</param>
         public static void LineStipple(int factor, ushort pattern)
        {
            PreGLCall();
            glLineStipple(factor, pattern);
            PostGLCall();
        }

		/// <summary>
		/// Set's the current width of lines.
		/// </summary>
		/// <param name="width">New line width to set.</param>
		 public static void LineWidth(float width)
		{
			PreGLCall();
			glLineWidth(width);
			PostGLCall();
		}

        /// <summary>
        /// Set the display-list base for glCallLists.
        /// </summary>
        /// <param name="listbase">Specifies an integer offset that will be added to glCallLists offsets to generate display-list names. The initial value is 0.</param>
		 public static void ListBase (uint listbase)
        {
            PreGLCall();
            glListBase(listbase);
            PostGLCall();
        }

		/// <summary>
		/// Call this function to load the identity matrix into the current matrix stack.
		/// </summary>
		 public static void LoadIdentity()
		{
			PreGLCall();
			glLoadIdentity();
			PostGLCall();
		}

        /// <summary>
        /// Replace the current matrix with the specified matrix.
        /// </summary>
        /// <param name="m">Specifies a pointer to 16 consecutive values, which are used as the elements of a 4x4 column-major matrix.</param>
		 public static void LoadMatrix( double []m)
        {
            PreGLCall();
            glLoadMatrixd(m);
            PreGLCall();
        }

        /// <summary>
        /// Replace the current matrix with the specified matrix.
        /// </summary>
        /// <param name="m">Specifies a pointer to 16 consecutive values, which are used as the elements of a 4x4 column-major matrix.</param>
         public static void LoadMatrixf(float[] m)
        {
            PreGLCall();
            glLoadMatrixf(m);
            PreGLCall();
        }

		/// <summary>
		/// This function replaces the name at the top of the selection names stack
		/// with 'name'.
		/// </summary>
		/// <param name="name">The name to replace it with.</param>
		 public static void LoadName (uint name)
		{
			PreGLCall();
			glLoadName(name);
			PostGLCall();
		}

        /// <summary>
        /// Specify a logical pixel operation for color index rendering.
        /// </summary>
        /// <param name="opcode">Specifies a symbolic constant	that selects a logical operation.</param>
		 public static void LogicOp (uint opcode)
        {
            PreGLCall();
            glLogicOp(opcode);
            PostGLCall();
        }

        /// <summary>
        /// Specify a logical pixel operation for color index rendering.
        /// </summary>
        /// <param name="logicOp">Specifies a symbolic constant	that selects a logical operation.</param>
         public static void LogicOp(LogicOp logicOp)
        {
            PreGLCall();
            glLogicOp((uint)logicOp);
            PostGLCall();
        }

		/// <summary>
		/// This function transforms the projection matrix so that it looks at a certain
		/// point, from a certain point.
		/// </summary>
		/// <param name="eyex">Position of the eye.</param>
		/// <param name="eyey">Position of the eye.</param>
		/// <param name="eyez">Position of the eye.</param>
		/// <param name="centerx">Point to look at.</param>
		/// <param name="centery">Point to look at.</param>
		/// <param name="centerz">Point to look at.</param>
		/// <param name="upx">'Up' Vector X Component.</param>
		/// <param name="upy">'Up' Vector Y Component.</param>
		/// <param name="upz">'Up' Vector Z Component.</param>
		 public static void LookAt(double eyex, double eyey, double eyez, 
			double centerx, double centery, double centerz, 
			double upx, double upy, double upz)
		{
			PreGLCall();
			gluLookAt(eyex, eyey, eyez, centerx, centery, centerz, upx, upy, upz);
			PostGLCall();
		}

		/// <summary>
		/// Defines a 1D evaluator.
		/// </summary>
		/// <param name="target">What the control points represent (e.g. MAP1_VERTEX_3).</param>
		/// <param name="u1">Range of the variable 'u'.</param>
		/// <param name="u2">Range of the variable 'u'.</param>
		/// <param name="stride">Offset between beginning of one control point, and beginning of next.</param>
		/// <param name="order">The degree plus one, should agree with the number of control points.</param>
		/// <param name="points">The data for the points.</param>
		 public static void Map1(uint target, double u1, double u2, int stride, int order,  double []points)
		{
			PreGLCall();
			glMap1d(target, u1, u2, stride, order, points);
			PostGLCall();
		}

		/// <summary>
		/// Defines a 1D evaluator.
		/// </summary>
		/// <param name="target">What the control points represent (e.g. MAP1_VERTEX_3).</param>
		/// <param name="u1">Range of the variable 'u'.</param>
		/// <param name="u2">Range of the variable 'u'.</param>
		/// <param name="stride">Offset between beginning of one control point, and beginning of next.</param>
		/// <param name="order">The degree plus one, should agree with the number of control points.</param>
		/// <param name="points">The data for the points.</param>
		 public static void Map1(uint target, float u1, float u2, int stride, int order,  float []points)
		{
			PreGLCall();
			glMap1f(target, u1, u2, stride, order, points);
			PostGLCall();
		}

		/// <summary>
		/// Defines a 2D evaluator.
		/// </summary>
		/// <param name="target">What the control points represent (e.g. MAP2_VERTEX_3).</param>
		/// <param name="u1">Range of the variable 'u'.</param>
		/// <param name="u2">Range of the variable 'u.</param>
		/// <param name="ustride">Offset between beginning of one control point and the next.</param>
		/// <param name="uorder">The degree plus one.</param>
		/// <param name="v1">Range of the variable 'v'.</param>
		/// <param name="v2">Range of the variable 'v'.</param>
		/// <param name="vstride">Offset between beginning of one control point and the next.</param>
		/// <param name="vorder">The degree plus one.</param>
		/// <param name="points">The data for the points.</param>
		 public static void Map2(uint target, double u1, double u2, int ustride, int uorder, double v1, double v2, int vstride, int vorder,  double []points)
		{
			PreGLCall();
			glMap2d(target, u1, u2, ustride, uorder, v1, v2, vstride, vorder, points);
			PostGLCall();
		}

		/// <summary>
		/// Defines a 2D evaluator.
		/// </summary>
		/// <param name="target">What the control points represent (e.g. MAP2_VERTEX_3).</param>
		/// <param name="u1">Range of the variable 'u'.</param>
		/// <param name="u2">Range of the variable 'u.</param>
		/// <param name="ustride">Offset between beginning of one control point and the next.</param>
		/// <param name="uorder">The degree plus one.</param>
		/// <param name="v1">Range of the variable 'v'.</param>
		/// <param name="v2">Range of the variable 'v'.</param>
		/// <param name="vstride">Offset between beginning of one control point and the next.</param>
		/// <param name="vorder">The degree plus one.</param>
		/// <param name="points">The data for the points.</param>
		 public static void Map2(uint target, float u1, float u2, int ustride, int uorder, float v1, float v2, int vstride, int vorder,  float []points)
		{
			PreGLCall();
			glMap2f(target, u1, u2, ustride, uorder, v1, v2, vstride, vorder, points);
			PostGLCall();
		}

		/// <summary>
		/// This function defines a grid that goes from u1 to u1 in n steps, evenly spaced.
		/// </summary>
		/// <param name="un">Number of steps.</param>
		/// <param name="u1">Range of variable 'u'.</param>
		/// <param name="u2">Range of variable 'u'.</param>
		 public static void MapGrid1(int un, double u1, double u2)
		{
			PreGLCall();
			glMapGrid1d(un, u1, u2);
			PostGLCall();
		}

		/// <summary>
		/// This function defines a grid that goes from u1 to u1 in n steps, evenly spaced.
		/// </summary>
		/// <param name="un">Number of steps.</param>
		/// <param name="u1">Range of variable 'u'.</param>
		/// <param name="u2">Range of variable 'u'.</param>
		 public static void MapGrid1(int un, float u1, float u2)
		{
			PreGLCall();
			glMapGrid1d(un, u1, u2);
			PostGLCall();
		}

		/// <summary>
		/// This function defines a grid that goes from u1 to u1 in n steps, evenly spaced,
		/// and the same for v.
		/// </summary>
		/// <param name="un">Number of steps.</param>
		/// <param name="u1">Range of variable 'u'.</param>
		/// <param name="u2">Range of variable 'u'.</param>
		/// <param name="vn">Number of steps.</param>
		/// <param name="v1">Range of variable 'v'.</param>
		/// <param name="v2">Range of variable 'v'.</param>
		 public static void MapGrid2(int un, double u1, double u2, int vn, double v1, double v2)
		{
			PreGLCall();
			glMapGrid2d(un, u1, u2, vn, v1, v2);
			PostGLCall();
		}

		/// <summary>
		/// This function defines a grid that goes from u1 to u1 in n steps, evenly spaced,
		/// and the same for v.
		/// </summary>
		/// <param name="un">Number of steps.</param>
		/// <param name="u1">Range of variable 'u'.</param>
		/// <param name="u2">Range of variable 'u'.</param>
		/// <param name="vn">Number of steps.</param>
		/// <param name="v1">Range of variable 'v'.</param>
		/// <param name="v2">Range of variable 'v'.</param>
		 public static void MapGrid2(int un, float u1, float u2, int vn, float v1, float v2)
		{
			PreGLCall();
			glMapGrid2f(un, u1, u2, vn, v1, v2);
			PostGLCall();
		}

		/// <summary>
		/// This function sets a material parameter.
		/// </summary>
		/// <param name="face">What faces is this parameter for (i.e front/back etc).</param>
		/// <param name="pname">What parameter you want to set.</param>
		/// <param name="param">The value to set 'pname' to.</param>
		 public static void Material(uint face, uint pname, float param)
		{
			PreGLCall();
			glMaterialf(face, pname, param);
			PostGLCall();
		}

		/// <summary>
		/// This function sets a material parameter.
		/// </summary>
		/// <param name="face">What faces is this parameter for (i.e front/back etc).</param>
		/// <param name="pname">What parameter you want to set.</param>
		/// <param name="parameters">The value to set 'pname' to.</param>
		 public static void Material(uint face, uint pname,  float[] parameters)
		{
			PreGLCall();
			glMaterialfv(face, pname, parameters);
			PostGLCall();
		}

		/// <summary>
		/// This function sets a material parameter.
		/// </summary>
		/// <param name="face">What faces is this parameter for (i.e front/back etc).</param>
		/// <param name="pname">What parameter you want to set.</param>
		/// <param name="param">The value to set 'pname' to.</param>
		 public static void Material(uint face, uint pname, int param)
		{
			PreGLCall();
			glMateriali(face, pname, param);
			PostGLCall();
		}

		/// <summary>
		/// This function sets a material parameter.
		/// </summary>
		/// <param name="face">What faces is this parameter for (i.e front/back etc).</param>
		/// <param name="pname">What parameter you want to set.</param>
		/// <param name="parameters">The value to set 'pname' to.</param>
		 public static void Material(uint face, uint pname,  int[] parameters)
		{
			PreGLCall();
			glMaterialiv(face, pname, parameters);
			PostGLCall();
		}

		/// <summary>
		/// Set the current matrix mode (the matrix that matrix operations will be 
		/// performed on).
		/// </summary>
		/// <param name="mode">The mode, normally PROJECTION or MODELVIEW.</param>
		 public static void MatrixMode (uint mode)
		{
			PreGLCall();
			glMatrixMode(mode);
			PostGLCall();
		}

		/// <summary>
		/// Set the current matrix mode (the matrix that matrix operations will be 
		/// performed on).
		/// </summary>
		/// <param name="mode">The mode, normally PROJECTION or MODELVIEW.</param>
         public static void MatrixMode(MatrixMode mode)
		{
			PreGLCall();
			glMatrixMode((uint)mode);
			PostGLCall();
		}

        /// <summary>
        /// Multiply the current matrix with the specified matrix.
        /// </summary>
        /// <param name="m">Points to 16 consecutive values that are used as the elements of a 4x4 column-major matrix.</param>
		 public static void MultMatrix( double []m)
        {
            PreGLCall();
            glMultMatrixd(m);
            PostGLCall();
        }

        /// <summary>
        /// Multiply the current matrix with the specified matrix.
        /// </summary>
        /// <param name="m">Points to 16 consecutive values that are used as the elements of a 4x4 column-major matrix.</param>
		 public static void MultMatrix( float []m)
        {
            PreGLCall();
            glMultMatrixf(m);
            PostGLCall();
        }

		/// <summary>
		/// This function starts compiling a new display list.
		/// </summary>
		/// <param name="list">The list to compile.</param>
		/// <param name="mode">Either COMPILE or COMPILE_AND_EXECUTE.</param>
		 public static void NewList(uint list, uint mode)
		{
			PreGLCall();
			glNewList(list, mode);
			PostGLCall();
		}

		/// <summary>
		/// This function creates a new glu NURBS renderer object.
		/// </summary>
		/// <returns>A Pointer to the NURBS renderer.</returns>
		public static IntPtr NewNurbsRenderer()
		{
			PreGLCall();
			IntPtr nurbs = gluNewNurbsRenderer();
			PostGLCall();

			return nurbs;
		}

		/// <summary>
		/// This function creates a new OpenGL Quadric Object.
		/// </summary>
		/// <returns>The pointer to the Quadric Object.</returns>
		public static IntPtr NewQuadric()
		{
			PreGLCall();
			IntPtr quad = gluNewQuadric();
			PostGLCall();

			return quad;
		}

        /// <summary>
        /// Set the current normal.
        /// </summary>
        /// <param name="nx">Normal Coordinate.</param>
        /// <param name="ny">Normal Coordinate.</param>
        /// <param name="nz">Normal Coordinate.</param>
		 public static void Normal(byte nx, byte ny, byte nz)
        {
            PreGLCall();
            glNormal3b(nx, ny, nz);
            PostGLCall();
        }

        /// <summary>
        /// This function sets the current normal.
        /// </summary>
        /// <param name="v">The normal.</param>
         public static void Normal(byte[] v)
        {
            PreGLCall();
            glNormal3bv(v);
            PostGLCall();
        }

        /// <summary>
        /// Set the current normal.
        /// </summary>
        /// <param name="nx">Normal Coordinate.</param>
        /// <param name="ny">Normal Coordinate.</param>
        /// <param name="nz">Normal Coordinate.</param>
         public static void Normal(double nx, double ny, double nz)
        {
            PreGLCall();
            glNormal3d(nx, ny, nz);
            PostGLCall();
        }

        /// <summary>
        /// This function sets the current normal.
        /// </summary>
        /// <param name="v">The normal.</param>
         public static void Normal(double[] v)
        {
            PreGLCall();
            glNormal3dv(v);
            PostGLCall();
        }

        /// <summary>
        /// Set the current normal.
        /// </summary>
        /// <param name="nx">Normal Coordinate.</param>
        /// <param name="ny">Normal Coordinate.</param>
        /// <param name="nz">Normal Coordinate.</param>
         public static void Normal(float nx, float ny, float nz)
        {
            PreGLCall();
            glNormal3f(nx, ny, nz);
            PostGLCall();
        }

		/// <summary>
		/// This function sets the current normal.
		/// </summary>
		/// <param name="v">The normal.</param>
		 public static void Normal(float[] v)
		{
			PreGLCall();
			glNormal3fv(v);
			PostGLCall();
        }

        /// <summary>
        /// Set the current normal.
        /// </summary>
        /// <param name="nx">Normal Coordinate.</param>
        /// <param name="ny">Normal Coordinate.</param>
        /// <param name="nz">Normal Coordinate.</param>
         public static void Normal3i(int nx, int ny, int nz)
        {
            PreGLCall();
            glNormal3i(nx, ny, nz);
            PostGLCall();
        }

        /// <summary>
        /// This function sets the current normal.
        /// </summary>
        /// <param name="v">The normal.</param>
         public static void Normal(int[] v)
        {
            PreGLCall();
            glNormal3iv(v);
            PostGLCall();
        }

        /// <summary>
        /// Set the current normal.
        /// </summary>
        /// <param name="nx">Normal Coordinate.</param>
        /// <param name="ny">Normal Coordinate.</param>
        /// <param name="nz">Normal Coordinate.</param>
         public static void Normal(short nx, short ny, short nz)
        {
            PreGLCall();
            glNormal3s(nx, ny, nz);
            PostGLCall();
        }

        /// <summary>
        /// This function sets the current normal.
        /// </summary>
        /// <param name="v">The normal.</param>
         public static void Normal(short[] v)
        {
            PreGLCall();
            glNormal3sv(v);
            PostGLCall();
        }

        /// <summary>
        /// Set's the pointer to the normal array.
        /// </summary>
        /// <param name="type">The type of data.</param>
        /// <param name="stride">The space in bytes between each normal.</param>
        /// <param name="pointer">The normals.</param>
         public static void NormalPointer(uint type, int stride, IntPtr pointer)
        {
            PreGLCall();
            glNormalPointer(type, stride, pointer);
            PostGLCall();
        }

		/// <summary>
		/// Set's the pointer to the normal array.
		/// </summary>
		/// <param name="type">The type of data.</param>
		/// <param name="stride">The space in bytes between each normal.</param>
		/// <param name="pointer">The normals.</param>
		 public static void NormalPointer(uint type, int stride, float[] pointer)
		{
			PreGLCall();
			glNormalPointer(type, stride, pointer);
			PostGLCall();
		}

		/// <summary>
		/// This function defines a NURBS Curve.
		/// </summary>
		/// <param name="nurbsObject">The NURBS object.</param>
		/// <param name="knotsCount">The number of knots.</param>
		/// <param name="knots">The knots themselves.</param>
		/// <param name="stride">The stride, i.e. distance between vertices in the 
		/// control points array.</param>
		/// <param name="controlPointsArray">The array of control points.</param>
		/// <param name="order">The order of the polynomial.</param>
		/// <param name="type">The type of data to generate.</param>
		 public static void NurbsCurve(IntPtr nurbsObject, int knotsCount, float[] knots, 
			int stride, float[] controlPointsArray, int order, uint type)
		{
			PreGLCall();
			gluNurbsCurve(nurbsObject, knotsCount, knots, stride, controlPointsArray,
				order, type);
			PostGLCall();
		}

		/// <summary>
		/// This function sets a NURBS property.
		/// </summary>
		/// <param name="nurbsObject">The object to set the property for.</param>
		/// <param name="property">The property to set.</param>
		/// <param name="value">The new value of the property.</param>
		 public static void NurbsProperty(IntPtr nurbsObject, int property, float value)
		{
			PreGLCall();
			gluNurbsProperty(nurbsObject, property, value);
			PostGLCall();
		}

    /// <summary>
    /// This function defines a NURBS surface.
    /// </summary>
    /// <param name="nurbsObject">The NURBS object.</param>
    /// <param name="sknotsCount">The sknots count.</param>
    /// <param name="sknots">The s-knots.</param>
    /// <param name="tknotsCount">The number of t-knots.</param>
    /// <param name="tknots">The t-knots.</param>
    /// <param name="sStride">The distance between s vertices.</param>
    /// <param name="tStride">The distance between t vertices.</param>
    /// <param name="controlPointsArray">The control points.</param>
    /// <param name="sOrder">The order of the s polynomial.</param>
    /// <param name="tOrder">The order of the t polynomial.</param>
    /// <param name="type">The type of data to generate.</param>
		 public static void NurbsSurface(IntPtr nurbsObject, int sknotsCount, float[] sknots, 
			int tknotsCount, float[] tknots, int sStride, int tStride, 
			float[] controlPointsArray, int sOrder, int tOrder, uint type)
		{
			PreGLCall();
			gluNurbsSurface(nurbsObject, sknotsCount, sknots, tknotsCount, tknots,
				sStride, tStride, controlPointsArray, sOrder, tOrder, type);
			PostGLCall();
		}
		
		/// <summary>
		/// This function creates an orthographic projection matrix (i.e one with no 
		/// perspective) and multiplies it to the current matrix stack, which would
		/// normally be 'PROJECTION'.
		/// </summary>
		/// <param name="left">Left clipping plane.</param>
		/// <param name="right">Right clipping plane.</param>
		/// <param name="bottom">Bottom clipping plane.</param>
		/// <param name="top">Top clipping plane.</param>
		/// <param name="zNear">Near clipping plane.</param>
		/// <param name="zFar">Far clipping plane.</param>
		 public static void Ortho(double left, double right, double bottom, 
			double top, double zNear, double zFar)
		{
			PreGLCall();
			glOrtho(left, right, bottom, top, zNear, zFar);
			PostGLCall();
		}
		/// <summary>
		/// This function creates an orthographic project based on a screen size.
		/// </summary>
		/// <param name="left">Left of the screen. (Normally 0).</param>
		/// <param name="right">Right of the screen.(Normally width).</param>
		/// <param name="bottom">Bottom of the screen (normally 0).</param>
		/// <param name="top">Top of the screen (normally height).</param>
		 public static void Ortho2D(double left, double right, double bottom, double top)
		{
			PreGLCall();
			gluOrtho2D(left, right, bottom, top);
			PostGLCall();
		}

		/// <summary>
		/// This function draws a partial disk from the quadric object.
		/// </summary>
		/// <param name="qobj">The Quadric objec.t</param>
		/// <param name="innerRadius">Radius of the inside of the disk.</param>
		/// <param name="outerRadius">Radius of the outside of the disk.</param>
		/// <param name="slices">The slices.</param>
		/// <param name="loops">The loops.</param>
		/// <param name="startAngle">Starting angle.</param>
		/// <param name="sweepAngle">Sweep angle.</param>
		 public static void PartialDisk(IntPtr qobj,double innerRadius,double outerRadius, int slices, int loops, double startAngle, double sweepAngle)
		{
			PreGLCall();
			gluPartialDisk(qobj, innerRadius, outerRadius, slices, loops, startAngle, sweepAngle);
			PostGLCall();
		}

        /// <summary>
        /// Place a marker in the feedback buffer.
        /// </summary>
        /// <param name="token">Specifies a marker value to be placed in the feedback buffer following a OpenGL.PASS_THROUGH_TOKEN.</param>
		 public static void PassThrough (float token)
        {
            PreGLCall();
            glPassThrough(token);
            PostGLCall();
        }

		/// <summary>
		/// This function creates a perspective matrix and multiplies it to the current
		/// matrix stack (which in most cases should be 'PROJECTION').
		/// </summary>
		/// <param name="fovy">Field of view angle (human eye = 60 Degrees).</param>
		/// <param name="aspect">Apsect Ratio (width of screen divided by height of screen).</param>
		/// <param name="zNear">Near clipping plane (normally 1).</param>
		/// <param name="zFar">Far clipping plane.</param>
		 public static void Perspective(double fovy, double aspect, double zNear, double zFar)
		{
			PreGLCall();
			gluPerspective(fovy, aspect, zNear, zFar);
			PostGLCall();
		}

		/// <summary>
		/// This function creates a 'pick matrix' normally used for selecting objects that
		/// are at a certain point on the screen.
		/// </summary>
		/// <param name="x">X Point.</param>
		/// <param name="y">Y Point.</param>
		/// <param name="width">Width of point to test (4 is normal).</param>
		/// <param name="height">Height of point to test (4 is normal).</param>
		/// <param name="viewport">The current viewport.</param>
		 public static void PickMatrix(double x, double y, double width, double height, int[] viewport)
		{
			PreGLCall();
			gluPickMatrix(x, y, width, height, viewport);
			PostGLCall();
		}

        /// <summary>
        /// Set up pixel transfer maps.
        /// </summary>
        /// <param name="map">Specifies a symbolic	map name.</param>
        /// <param name="mapsize">Specifies the size of the map being defined.</param>
        /// <param name="values">Specifies an	array of mapsize values.</param>
		 public static void PixelMap(uint map, int mapsize,  float[] values)
        {
            PreGLCall();
            glPixelMapfv(map, mapsize, values);
            PostGLCall();
        }

        /// <summary>
        /// Set up pixel transfer maps.
        /// </summary>
        /// <param name="map">Specifies a symbolic	map name.</param>
        /// <param name="mapsize">Specifies the size of the map being defined.</param>
        /// <param name="values">Specifies an	array of mapsize values.</param>
         public static void PixelMap(uint map, int mapsize, uint[] values)
        {
            PreGLCall();
            glPixelMapuiv(map, mapsize, values);
            PostGLCall();
        }

        /// <summary>
        /// Set up pixel transfer maps.
        /// </summary>
        /// <param name="map">Specifies a symbolic	map name.</param>
        /// <param name="mapsize">Specifies the size of the map being defined.</param>
        /// <param name="values">Specifies an	array of mapsize values.</param>
         public static void PixelMap(uint map, int mapsize, ushort[] values)
        {
            PreGLCall();
            glPixelMapusv(map, mapsize, values);
            PostGLCall();
        }

        /// <summary>
        /// Set pixel storage modes.
        /// </summary>
        /// <param name="pname">Specifies the symbolic	name of	the parameter to be set.</param>
        /// <param name="param">Specifies the value that pname	is set to.</param>
		 public static void PixelStore(uint pname, float param)
        {
            PreGLCall();
            glPixelStoref(pname, param);
            PostGLCall();
        }

        /// <summary>
        /// Set pixel storage modes.
        /// </summary>
        /// <param name="pname">Specifies the symbolic	name of	the parameter to be set.</param>
        /// <param name="param">Specifies the value that pname	is set to.</param>
         public static void PixelStore(uint pname, int param)
        {
            PreGLCall();
            glPixelStorei(pname, param);
            PostGLCall();
        }

        /// <summary>
        /// Set pixel transfer modes.
        /// </summary>
        /// <param name="pname">Specifies the symbolic name of the pixel transfer parameter to be set.</param>
        /// <param name="param">Specifies the value that pname is set to.</param>
         public static void PixelTransfer(uint pname, bool param)
        {
            PreGLCall();
            int p = param ? 1 : 0;
            glPixelTransferi(pname, p);
            PostGLCall();
        }

        /// <summary>
        /// Set pixel transfer modes.
        /// </summary>
        /// <param name="pname">Specifies the symbolic name of the pixel transfer parameter to be set.</param>
        /// <param name="param">Specifies the value that pname is set to.</param>
         public static void PixelTransfer(PixelTransferParameterName pname, bool param)
        {
            PreGLCall();
            int p = param ? 1 : 0;
            glPixelTransferi((uint)pname, p);
            PostGLCall();
        }

        /// <summary>
        /// Set pixel transfer modes.
        /// </summary>
        /// <param name="pname">Specifies the symbolic name of the pixel transfer parameter to be set.</param>
        /// <param name="param">Specifies the value that pname is set to.</param>
		 public static void PixelTransfer(uint pname, float param)
        {
            PreGLCall();
            glPixelTransferf(pname, param);
            PostGLCall();
        }

        /// <summary>
        /// Set pixel transfer modes.
        /// </summary>
        /// <param name="pname">Specifies the symbolic name of the pixel transfer parameter to be set.</param>
        /// <param name="param">Specifies the value that pname is set to.</param>
         public static void PixelTransfer(PixelTransferParameterName pname, float param)
        {
            PreGLCall();
            glPixelTransferf((uint)pname, param);
            PostGLCall();
        }

        /// <summary>
        /// Set pixel transfer modes.
        /// </summary>
        /// <param name="pname">Specifies the symbolic name of the pixel transfer parameter to be set.</param>
        /// <param name="param">Specifies the value that pname is set to.</param>
         public static void PixelTransfer(uint pname, int param)
        {
            PreGLCall();
            glPixelTransferi(pname, param);
            PostGLCall();
        }

        /// <summary>
        /// Set pixel transfer modes.
        /// </summary>
        /// <param name="pname">Specifies the symbolic name of the pixel transfer parameter to be set.</param>
        /// <param name="param">Specifies the value that pname is set to.</param>
         public static void PixelTransfer(PixelTransferParameterName pname, int param)
        {
            PreGLCall();
            glPixelTransferi((uint)pname, param);
            PostGLCall();
        }

        /// <summary>
        /// Specify	the pixel zoom factors.
        /// </summary>
        /// <param name="xfactor">Specify the x and y zoom factors for pixel write operations.</param>
        /// <param name="yfactor">Specify the x and y zoom factors for pixel write operations.</param>
		 public static void PixelZoom (float xfactor, float yfactor)
        {
            PreGLCall();
            glPixelZoom(xfactor, yfactor);
            PostGLCall();
        }

		/// <summary>
		/// The size of points to be rasterised.
		/// </summary>
		/// <param name="size">Size in pixels.</param>
		 public static void PointSize(float size)
		{
			PreGLCall();
			glPointSize(size);
			PostGLCall();
		}

		/// <summary>
		/// This sets the current drawing mode of polygons (points, lines, filled).
		/// </summary>
		/// <param name="face">The faces this applies to (front, back or both).</param>
		/// <param name="mode">The mode to set to (points, lines, or filled).</param>
		 public static void PolygonMode(uint face, uint mode)
		{
			PreGLCall();
			glPolygonMode(face, mode);
			PostGLCall();
		}

        /// <summary>
        /// This sets the current drawing mode of polygons (points, lines, filled).
        /// </summary>
        /// <param name="face">The faces this applies to (front, back or both).</param>
        /// <param name="mode">The mode to set to (points, lines, or filled).</param>
         public static void PolygonMode(FaceMode face, PolygonMode mode)
        {
            PreGLCall();
            glPolygonMode((uint)face, (uint)mode);
            PostGLCall();
        }

        /// <summary>
        /// Set	the scale and units used to calculate depth	values.
        /// </summary>
        /// <param name="factor">Specifies a scale factor that	is used	to create a variable depth offset for each polygon. The initial value is 0.</param>
        /// <param name="units">Is multiplied by an implementation-specific value to create a constant depth offset. The initial value is 0.</param>
		 public static void PolygonOffset (float factor, float units)
        {
            PreGLCall();
            glPolygonOffset(factor, units);
            PostGLCall();
        }

        /// <summary>
        /// Set the polygon stippling pattern.
        /// </summary>
        /// <param name="mask">Specifies a pointer to a 32x32 stipple pattern that will be unpacked from memory in the same way that glDrawPixels unpacks pixels.</param>
		 public static void PolygonStipple ( byte []mask)
        {
            PreGLCall();
            glPolygonStipple(mask);
            PostGLCall();
        }

		/// <summary>
		/// This function restores the attribute stack to the state it was when
		/// PushAttrib was called.
		/// </summary>
		 public static void PopAttrib()
		{
			PreGLCall();
			glPopAttrib();
			PostGLCall();
		}

        /// <summary>
        /// Pop the client attribute stack.
        /// </summary>
		 public static void PopClientAttrib ()
        {
            PreGLCall();
            glPopClientAttrib();
            PostGLCall();
        }

		/// <summary>
		/// Restore the previously saved state of the current matrix stack.
		/// </summary>
		 public static void PopMatrix()
		{            
            PreGLCall();
			glPopMatrix();
			PostGLCall();
		}

		/// <summary>
		/// This takes the top name off the selection names stack.
		/// </summary>
		 public static void PopName()
		{
			PreGLCall();
			glPopName();
			PostGLCall();
		}

        /// <summary>
        /// Set texture residence priority.
        /// </summary>
        /// <param name="n">Specifies the number of textures to be prioritized.</param>
        /// <param name="textures">Specifies an array containing the names of the textures to be prioritized.</param>
        /// <param name="priorities">Specifies	an array containing the	texture priorities. A priority given in an element of priorities applies to the	texture	named by the corresponding element of textures.</param>
		 public static void PrioritizeTextures (int n,  uint []textures,  float []priorities)
        {
            PreGLCall();
            glPrioritizeTextures(n, textures, priorities);
            PostGLCall();
        }

		/// <summary>
		/// This function Maps the specified object coordinates into window coordinates.
		/// </summary>
		/// <param name="objx">The object's x coord.</param>
		/// <param name="objy">The object's y coord.</param>
		/// <param name="objz">The object's z coord.</param>
		/// <param name="modelMatrix">The modelview matrix.</param>
		/// <param name="projMatrix">The projection matrix.</param>
		/// <param name="viewport">The viewport.</param>
		/// <param name="winx">The window x coord.</param>
		/// <param name="winy">The Window y coord.</param>
		/// <param name="winz">The Window z coord.</param>
		 public static void Project(double objx, double objy, double objz, double[] modelMatrix, double[] projMatrix, int[] viewport, double[] winx, double[] winy, double[] winz)
		{
			PreGLCall();
			gluProject(objx, objy, objz, modelMatrix, projMatrix, viewport, winx, winy, winz);
			PostGLCall();
		}		

		/// <summary>
		/// Save the current state of the attribute groups specified by 'mask'.
		/// </summary>
		/// <param name="mask">The attibute groups to save.</param>
		 public static void PushAttrib(uint mask)
		{
			PreGLCall();
			glPushAttrib(mask);
			PostGLCall();
		}

        /// <summary>
        /// Save the current state of the attribute groups specified by 'mask'.
        /// </summary>
        /// <param name="mask">The attibute groups to save.</param>
         public static void PushAttrib(AttributeMask mask)
        {
            PreGLCall();
            glPushAttrib((uint)mask);
            PostGLCall();
        }

        /// <summary>
        /// Push the client attribute stack.
        /// </summary>
        /// <param name="mask">Specifies a mask that indicates	which attributes to save.</param>
		 public static void PushClientAttrib (uint mask)
        {
            PreGLCall();
            glPushClientAttrib(mask);
            PostGLCall();
        }

		/// <summary>
		/// Save the current state of the current matrix stack.
		/// </summary>
		 public static void PushMatrix()
		{
			PreGLCall();
			glPushMatrix();
			PostGLCall();
		}

		/// <summary>
		/// This function adds a new name to the selection buffer.
		/// </summary>
		/// <param name="name">The name to add.</param>
		 public static void PushName(uint name)
		{
			PreGLCall();
			glPushName(name);
			PostGLCall();
		}

		/// <summary>
		/// This set's the Generate Normals propery of the specified Quadric object.
		/// </summary>
		/// <param name="quadricObject">The quadric object.</param>
		/// <param name="normals">The type of normals to generate.</param>
		 public static void QuadricNormals(IntPtr quadricObject, uint normals)
		{
			PreGLCall();
			gluQuadricNormals(quadricObject, normals);
			PostGLCall();
		}

		/// <summary>
		/// This function sets the type of texture coordinates being generated by
		/// the specified quadric object.
		/// </summary>
		/// <param name="quadricObject">The quadric object.</param>
		/// <param name="textureCoords">The type of coordinates to generate.</param>
		 public static void QuadricTexture(IntPtr quadricObject, int textureCoords)
		{
			PreGLCall();
			gluQuadricTexture(quadricObject, textureCoords);
			PostGLCall();
		}

		/// <summary>
		/// This sets the orientation for the quadric object.
		/// </summary>
		/// <param name="quadricObject">The quadric object.</param>
		/// <param name="orientation">The orientation.</param>
		 public static void QuadricOrientation(IntPtr quadricObject, int orientation)
		{
			PreGLCall();
			gluQuadricOrientation(quadricObject, orientation);
			PostGLCall();
		}

		/// <summary>
		/// This sets the current drawstyle for the Quadric Object.
		/// </summary>
		/// <param name="quadObject">The quadric object.</param>
		/// <param name="drawStyle">The draw style.</param>
		 public static void QuadricDrawStyle (IntPtr quadObject, uint drawStyle)
		{
			PreGLCall();
			gluQuadricDrawStyle(quadObject, drawStyle);
			PostGLCall();
		}

        /// <summary>
        /// This function sets the current raster position.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
         public static void RasterPos(double x, double y)
        {
            PreGLCall();
            glRasterPos2d(x, y);
            PostGLCall();
        }

        /// <summary>
        /// This function sets the current raster position.
        /// </summary>
        /// <param name="v">The coordinate.</param>
         public static void RasterPos(double[] v) 
        {
            PreGLCall();
            if (v.Length == 2)
                glRasterPos2dv(v);
            else if (v.Length == 3)
                glRasterPos3dv(v);
            else
                glRasterPos4dv(v);
            PostGLCall();
        }

        /// <summary>
        /// This function sets the current raster position.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
         public static void RasterPos(float x, float y)
        {
            PreGLCall();
            glRasterPos2f(x, y);
            PostGLCall();
        }

        /// <summary>
        /// This function sets the current raster position.
        /// </summary>
        /// <param name="v">The coordinate.</param>
         public static void RasterPos(float[] v)
        {
            PreGLCall();
            if (v.Length == 2)
                glRasterPos2fv(v);
            else if (v.Length == 3)
                glRasterPos3fv(v);
            else
                glRasterPos4fv(v);
            PostGLCall();
        }

		/// <summary>
		/// This function sets the current raster position.
		/// </summary>
		/// <param name="x">X coordinate.</param>
		/// <param name="y">Y coordinate.</param>
		 public static void RasterPos(int x, int y)
		{
			PreGLCall();
			glRasterPos2i(x, y);
			PostGLCall();
        }

        /// <summary>
        /// This function sets the current raster position.
        /// </summary>
        /// <param name="v">The coordinate.</param>
         public static void RasterPos(int[] v)
        {
            PreGLCall();
            if (v.Length == 2)
                glRasterPos2iv(v);
            else if (v.Length == 3)
                glRasterPos3iv(v);
            else
                glRasterPos4iv(v);
            PostGLCall();
        }

        /// <summary>
        /// This function sets the current raster position.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
         public static void RasterPos(short x, short y)
        {
            PreGLCall();
            glRasterPos2s(x, y);
            PostGLCall();
        }

        /// <summary>
        /// This function sets the current raster position.
        /// </summary>
        /// <param name="v">The coordinate.</param>
         public static void RasterPos(short[] v)
        {
            PreGLCall();
            if (v.Length == 2)
                glRasterPos2sv(v);
            else if (v.Length == 3)
                glRasterPos3sv(v);
            else
                glRasterPos4sv(v);
            PostGLCall();
        }

        /// <summary>
        /// This function sets the current raster position.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="z">Z coordinate.</param>
         public static void RasterPos(double x, double y, double z)
        {
            PreGLCall();
            glRasterPos3d(x, y, z);
            PostGLCall();
        }

        /// <summary>
        /// This function sets the current raster position.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="z">Z coordinate.</param>
         public static void RasterPos(float x, float y, float z)
        {
            PreGLCall();
            glRasterPos3f(x, y, z);
            PostGLCall();
        }

        /// <summary>
        /// This function sets the current raster position.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="z">Z coordinate.</param>
         public static void RasterPos(int x, int y, int z)
        {
            PreGLCall();
            glRasterPos3i(x, y, z);
            PostGLCall();
        }

        /// <summary>
        /// This function sets the current raster position.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="z">Z coordinate.</param>
         public static void RasterPos(short x, short y, short z)
        {
            PreGLCall();
            glRasterPos3s(x, y, z);
            PostGLCall();
        }

        /// <summary>
        /// This function sets the current raster position.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="z">Z coordinate.</param>
        /// <param name="w">W coordinate.</param>
         public static void RasterPos(double x, double y, double z, double w)
        {
            PreGLCall();
            glRasterPos4d(x, y, z, w);
            PostGLCall();
        }

        /// <summary>
        /// This function sets the current raster position.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="z">Z coordinate.</param>
        /// <param name="w">W coordinate.</param>
         public static void RasterPos(float x, float y, float z, float w)
        {
            PreGLCall();
            glRasterPos4f(x, y, z, w);
            PostGLCall();
        }

        /// <summary>
        /// This function sets the current raster position.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="z">Z coordinate.</param>
        /// <param name="w">W coordinate.</param>
         public static void RasterPos(int x, int y, int z, int w)
        {
            PreGLCall();
            glRasterPos4i(x, y, z, w);
            PostGLCall();
        }

        /// <summary>
        /// This function sets the current raster position.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="z">Z coordinate.</param>
        /// <param name="w">W coordinate.</param>
         public static void RasterPos(short x, short y, short z, short w)
        {
            PreGLCall();
            glRasterPos4s(x, y, z, w);
            PostGLCall();
        }

        /// <summary>
        /// Select	a color	buffer source for pixels.
        /// </summary>
        /// <param name="mode">Specifies a color buffer.  Accepted values are OpenGL.FRONT_LEFT, OpenGL.FRONT_RIGHT, OpenGL.BACK_LEFT, OpenGL.BACK_RIGHT, OpenGL.FRONT, OpenGL.BACK, OpenGL.LEFT, OpenGL.GL_RIGHT, and OpenGL.AUXi, where i is between 0 and OpenGL.AUX_BUFFERS - 1.</param>
		 public static void ReadBuffer(uint mode)
        {
            PreGLCall();
            glReadBuffer(mode);
            PostGLCall();
        }

        /// <summary>
        /// Reads a block of pixels from the frame buffer.
        /// </summary>
        /// <param name="x">Top-Left X value.</param>
        /// <param name="y">Top-Left Y value.</param>
        /// <param name="width">Width of block to read.</param>
        /// <param name="height">Height of block to read.</param>
        /// <param name="format">Specifies the format of the pixel data. The following symbolic values are accepted: OpenGL.COLOR_INDEX, OpenGL.STENCIL_INDEX, OpenGL.DEPTH_COMPONENT, OpenGL.RED, OpenGL.GREEN, OpenGL.BLUE, OpenGL.ALPHA, OpenGL.RGB, OpenGL.RGBA, OpenGL.LUMINANCE and OpenGL.LUMINANCE_ALPHA.</param>
        /// <param name="type">Specifies the data type of the pixel data.Must be one of OpenGL.UNSIGNED_BYTE, OpenGL.BYTE, OpenGL.BITMAP, OpenGL.UNSIGNED_SHORT, OpenGL.SHORT, OpenGL.UNSIGNED_INT, OpenGL.INT or OpenGL.FLOAT.</param>
        /// <param name="pixels">Storage for the pixel data received.</param>
		 public static void ReadPixels(int x, int y, int width, int height, uint format, 
            uint type, byte[] pixels)
        {
            PreGLCall();
            glReadPixels(x, y, width, height, format, type, pixels);
            PostGLCall();
        }

        /// <summary>
        /// Reads a block of pixels from the frame buffer.
        /// </summary>
        /// <param name="x">Top-Left X value.</param>
        /// <param name="y">Top-Left Y value.</param>
        /// <param name="width">Width of block to read.</param>
        /// <param name="height">Height of block to read.</param>
        /// <param name="format">Specifies the format of the pixel data. The following symbolic values are accepted: OpenGL.COLOR_INDEX, OpenGL.STENCIL_INDEX, OpenGL.DEPTH_COMPONENT, OpenGL.RED, OpenGL.GREEN, OpenGL.BLUE, OpenGL.ALPHA, OpenGL.RGB, OpenGL.RGBA, OpenGL.LUMINANCE and OpenGL.LUMINANCE_ALPHA.</param>
        /// <param name="type">Specifies the data type of the pixel data.Must be one of OpenGL.UNSIGNED_BYTE, OpenGL.BYTE, OpenGL.BITMAP, OpenGL.UNSIGNED_SHORT, OpenGL.SHORT, OpenGL.UNSIGNED_INT, OpenGL.INT or OpenGL.FLOAT.</param>
        /// <param name="pixels">Storage for the pixel data received.</param>
         public static void ReadPixels(int x, int y, int width, int height, uint format,
            uint type, IntPtr pixels)
        {
            PreGLCall();
            glReadPixels(x, y, width, height, format, type, pixels);
            PostGLCall();
        }

		/// <summary>
		/// Draw a rectangle from two coordinates (top-left and bottom-right).
		/// </summary>
		/// <param name="x1">Top-Left X value.</param>
		/// <param name="y1">Top-Left Y value.</param>
		/// <param name="x2">Bottom-Right X Value.</param>
		/// <param name="y2">Bottom-Right Y Value.</param>
		 public static void Rect(double x1, double y1, double x2, double y2)
		{
			PreGLCall();
			glRectd(x1, y1, x2, y2);
			PostGLCall();
		}

		/// <summary>
		/// Draw a rectangle from two coordinates, expressed as arrays, e.g
		/// Rect(new float[] {0, 0}, new float[] {10, 10});
		/// </summary>
		/// <param name="v1">Top-Left point.</param>
		/// <param name="v2">Bottom-Right point.</param>
		 public static void Rect( double []v1,  double []v2)
		{
			PreGLCall();
			glRectdv(v1, v2);
			PostGLCall();
		}

		/// <summary>
		/// Draw a rectangle from two coordinates (top-left and bottom-right).
		/// </summary>
		/// <param name="x1">Top-Left X value.</param>
		/// <param name="y1">Top-Left Y value.</param>
		/// <param name="x2">Bottom-Right X Value.</param>
		/// <param name="y2">Bottom-Right Y Value.</param>
		 public static void Rect(float x1, float y1, float x2, float y2)
		{
			PreGLCall();
			glRectd(x1, y1, x2, y2);
			PostGLCall();
		}

		/// <summary>
		/// Draw a rectangle from two coordinates, expressed as arrays, e.g
		/// Rect(new float[] {0, 0}, new float[] {10, 10});
		/// </summary>
		/// <param name="v1">Top-Left point.</param>
		/// <param name="v2">Bottom-Right point.</param>
		 public static void Rect(float []v1,  float []v2)
		{
			PreGLCall();
			glRectfv(v1, v2);
			PostGLCall();
		}

		/// <summary>
		/// Draw a rectangle from two coordinates (top-left and bottom-right).
		/// </summary>
		/// <param name="x1">Top-Left X value.</param>
		/// <param name="y1">Top-Left Y value.</param>
		/// <param name="x2">Bottom-Right X Value.</param>
		/// <param name="y2">Bottom-Right Y Value.</param>
		 public static void Rect(int x1, int y1, int x2, int y2)
		{
			PreGLCall();
			glRecti(x1, y1, x2, y2);
			PostGLCall();
		}

		/// <summary>
		/// Draw a rectangle from two coordinates, expressed as arrays, e.g
		/// Rect(new float[] {0, 0}, new float[] {10, 10});
		/// </summary>
		/// <param name="v1">Top-Left point.</param>
		/// <param name="v2">Bottom-Right point.</param>
		 public static void Rect( int []v1,  int []v2)
		{
			PreGLCall();
			glRectiv(v1, v2);
			PostGLCall();
		}

		/// <summary>
		/// Draw a rectangle from two coordinates (top-left and bottom-right).
		/// </summary>
		/// <param name="x1">Top-Left X value.</param>
		/// <param name="y1">Top-Left Y value.</param>
		/// <param name="x2">Bottom-Right X Value.</param>
		/// <param name="y2">Bottom-Right Y Value.</param>
		 public static void Rect(short x1, short y1, short x2, short y2)
		{
			PreGLCall();
			glRects(x1, y1, x2, y2);
			PostGLCall();
		}

		/// <summary>
		/// Draw a rectangle from two coordinates, expressed as arrays, e.g
		/// Rect(new float[] {0, 0}, new float[] {10, 10});
		/// </summary>
		/// <param name="v1">Top-Left point.</param>
		/// <param name="v2">Bottom-Right point.</param>
		 public static void Rect(short []v1, short []v2)
		{
			PreGLCall();
			glRectsv(v1, v2);
			PostGLCall();
		}

		/// <summary>
		/// This function sets the current render mode (render, feedback or select).
		/// </summary>
		/// <param name="mode">The Render mode (RENDER, SELECT or FEEDBACK).</param>
		/// <returns>The hits that selection or feedback caused..</returns>
		public static int RenderMode(uint mode)
		{
			PreGLCall();
			int hits = glRenderMode(mode);
			PostGLCall();
			return hits;
		}

        /// <summary>
        /// This function sets the current render mode (render, feedback or select).
        /// </summary>
        /// <param name="mode">The Render mode (RENDER, SELECT or FEEDBACK).</param>
        /// <returns>The hits that selection or feedback caused..</returns>
        public static int RenderMode(RenderingMode mode)
        {
            PreGLCall();
            int hits = glRenderMode((uint)mode);
            PostGLCall();
            return hits;
        }

		/// <summary>
		/// This function applies a rotation transformation to the current matrix.
		/// </summary>
		/// <param name="angle">The angle to rotate.</param>
		/// <param name="x">Amount along x.</param>
		/// <param name="y">Amount along y.</param>
		/// <param name="z">Amount along z.</param>
		 public static void Rotate(double angle, double x, double y, double z)
		{
			PreGLCall();
			glRotated(angle, x, y, z);
			PostGLCall();
		}

		/// <summary>
		/// This function applies a rotation transformation to the current matrix.
		/// </summary>
		/// <param name="angle">The angle to rotate.</param>
		/// <param name="x">Amount along x.</param>
		/// <param name="y">Amount along y.</param>
		/// <param name="z">Amount along z.</param>
		 public static void Rotate(float angle, float x, float y, float z)
		{
			PreGLCall();
			glRotatef(angle, x, y, z);
			PostGLCall();
		}

		/// <summary>
		/// This function quickly does three rotations, one about each axis, with the
		/// given angles (it's not an OpenGL function, but very useful).
		/// </summary>
		/// <param name="anglex">The angle to rotate about x.</param>
		/// <param name="angley">The angle to rotate about y.</param>
		/// <param name="anglez">The angle to rotate about z.</param>
		 public static void Rotate(float anglex, float angley, float anglez)
		{
			PreGLCall();
			glRotatef(anglex, 1, 0, 0);
			glRotatef(angley, 0, 1, 0);
			glRotatef(anglez, 0, 0, 1);
			PostGLCall();
		}

		/// <summary>
		/// This function applies a scale transformation to the current matrix.
		/// </summary>
		/// <param name="x">The amount to scale along x.</param>
		/// <param name="y">The amount to scale along y.</param>
		/// <param name="z">The amount to scale along z.</param>
		 public static void Scale(double x, double y, double z)
		{
			PreGLCall();
			glScaled(x, y, z);
			PostGLCall();
		}

		/// <summary>
		/// This function applies a scale transformation to the current matrix.
		/// </summary>
		/// <param name="x">The amount to scale along x.</param>
		/// <param name="y">The amount to scale along y.</param>
		/// <param name="z">The amount to scale along z.</param>
		 public static void Scale(float x, float y, float z)
		{
			PreGLCall();
			glScalef(x, y, z);
			PostGLCall();
		}

        /// <summary>
        /// Define the scissor box.
        /// </summary>
        /// <param name="x">Specify the lower left corner of the scissor box. Initially (0, 0).</param>
        /// <param name="y">Specify the lower left corner of the scissor box. Initially (0, 0).</param>
        /// <param name="width">Specify the width and height of the scissor box. When a GL context is first attached to a window, width and height are set to the dimensions of that window.</param>
        /// <param name="height">Specify the width and height of the scissor box. When a GL context is first attached to a window, width and height are set to the dimensions of that window.</param>
		 public static void Scissor (int x, int y, int width, int height)
        {
            PreGLCall();
            glScissor(x, y, width, height);
            PostGLCall();
        }

		/// <summary>
		/// This function sets the current select buffer.
		/// </summary>
		/// <param name="size">The size of the buffer you are passing.</param>
		/// <param name="buffer">The buffer itself.</param>
		 public static void SelectBuffer(int size, uint[] buffer)
		{
			PreGLCall();
			glSelectBuffer(size, buffer);
			PostGLCall();
		}

        /// <summary>
        /// Select flat or smooth shading.
        /// </summary>
        /// <param name="mode">Specifies a symbolic value representing a shading technique. Accepted values are OpenGL.FLAT and OpenGL.SMOOTH. The default is OpenGL.SMOOTH.</param>
		 public static void ShadeModel (uint mode)
        {
            PreGLCall();
            glShadeModel(mode);
            PostGLCall();
        }

        /// <summary>
        /// Select flat or smooth shading.
        /// </summary>
        /// <param name="mode">Specifies a symbolic value representing a shading technique. Accepted values are OpenGL.FLAT and OpenGL.SMOOTH. The default is OpenGL.SMOOTH.</param>
         public static void ShadeModel(ShadeModel mode)
        {
            PreGLCall();
            glShadeModel((uint)mode);
            PostGLCall();
        }

		/// <summary>
		/// This function draws a sphere from a Quadric Object.
		/// </summary>
		/// <param name="qobj">The quadric object.</param>
		/// <param name="radius">Sphere radius.</param>
		/// <param name="slices">Slices of the sphere.</param>
		/// <param name="stacks">Stakcs of the sphere.</param>
		 public static void Sphere(IntPtr qobj, double radius, int slices, int stacks)
		{
			PreGLCall();
			gluSphere(qobj, radius, slices, stacks);
			PostGLCall();
		}

		/// <summary>
		/// This function sets the current stencil buffer function.
		/// </summary>
		/// <param name="func">The function type.</param>
		/// <param name="reference">The function reference.</param>
		/// <param name="mask">The function mask.</param>
		 public static void StencilFunc(uint func, int reference, uint mask)
		{
			PreGLCall();
			glStencilFunc(func, reference, mask);
			PostGLCall();
		}

        /// <summary>
        /// This function sets the current stencil buffer function.
        /// </summary>
        /// <param name="func">The function type.</param>
        /// <param name="reference">The function reference.</param>
        /// <param name="mask">The function mask.</param>
         public static void StencilFunc(StencilFunction func, int reference, uint mask)
        {
            PreGLCall();
            glStencilFunc((uint)func, reference, mask);
            PostGLCall();
        }

		/// <summary>
		/// This function sets the stencil buffer mask.
		/// </summary>
		/// <param name="mask">The mask.</param>
		 public static void StencilMask(uint mask)
		{
			PreGLCall();
			glStencilMask(mask);
			PostGLCall();
		}

		/// <summary>
		/// This function sets the stencil buffer operation.
		/// </summary>
		/// <param name="fail">Fail operation.</param>
		/// <param name="zfail">Depth fail component.</param>
		/// <param name="zpass">Depth pass component.</param>
		 public static void StencilOp(uint fail, uint zfail, uint zpass)
		{
			PreGLCall();
			glStencilOp(fail, zfail, zpass);
			PostGLCall();
		}

		/// <summary>
		/// This function sets the stencil buffer operation.
		/// </summary>
		/// <param name="fail">Fail operation.</param>
		/// <param name="zfail">Depth fail component.</param>
		/// <param name="zpass">Depth pass component.</param>
         public static void StencilOp(StencilOperation fail, StencilOperation zfail, StencilOperation zpass)
		{
			PreGLCall();
			glStencilOp((uint)fail, (uint)zfail, (uint)zpass);
			PostGLCall();
		}

		/// <summary>
		/// This function sets the current texture coordinates.
		/// </summary>
		/// <param name="s">Texture Coordinate.</param>
		 public static void TexCoord(double s)
		{
			PreGLCall();
			glTexCoord1d(s);
			PostGLCall();
		}

		/// <summary>
		/// This function sets the current texture coordinates.
		/// </summary>
		/// <param name="v">Array of 1,2,3 or 4 Texture Coordinates.</param>
		 public static void TexCoord(double []v)
		{
			PreGLCall();
			if(v.Length == 1)
				glTexCoord1dv(v);
			else if(v.Length == 2)
				glTexCoord2dv(v);
			else if(v.Length == 3)
				glTexCoord3dv(v);
			else if(v.Length == 4)
				glTexCoord4dv(v);
			PostGLCall();
		}

		/// <summary>
		/// This function sets the current texture coordinates.
		/// </summary>
		/// <param name="s">Texture Coordinate.</param>
		 public static void TexCoord(float s)
		{
			PreGLCall();
			glTexCoord1f(s);
			PostGLCall();
		}

		/// <summary>
		/// This function sets the current texture coordinates. WARNING: if you
		/// can call something more explicit, like TexCoord2f then call that, it's
		/// much faster.
		/// </summary>
		/// <param name="v">Array of 1,2,3 or 4 Texture Coordinates.</param>
		 public static void TexCoord(float[] v)
		{
			PreGLCall();
			if(v.Length == 1)
				glTexCoord1fv(v);
			else if(v.Length == 2)
				glTexCoord2fv(v);
			else if(v.Length == 3)
				glTexCoord3fv(v);
			else if(v.Length == 4)
				glTexCoord4fv(v);
			PostGLCall();
		}

		/// <summary>
		/// This function sets the current texture coordinates.
		/// </summary>
		/// <param name="s">Texture Coordinate.</param>
		 public static void TexCoord(int s)
		{
			PreGLCall();
			glTexCoord1i(s);
			PostGLCall();
		}

		/// <summary>
		/// This function sets the current texture coordinates.
		/// </summary>
		/// <param name="v">Array of 1,2,3 or 4 Texture Coordinates.</param>
		 public static void TexCoord(int[] v)
		{
			PreGLCall();
			if(v.Length == 1)
				glTexCoord1iv(v);
			else if(v.Length == 2)
				glTexCoord2iv(v);
			else if(v.Length == 3)
				glTexCoord3iv(v);
			else if(v.Length == 4)
				glTexCoord4iv(v);
			PostGLCall();
		}

		/// <summary>
		/// This function sets the current texture coordinates.
		/// </summary>
		/// <param name="s">Texture Coordinate.</param>
		 public static void TexCoord(short s)
		{
			PreGLCall();
			glTexCoord1s(s);
			PostGLCall();
		}

		/// <summary>
		/// This function sets the current texture coordinates.
		/// </summary>
		/// <param name="v">Array of 1,2,3 or 4 Texture Coordinates.</param>
		 public static void TexCoord(short[] v)
		{
			PreGLCall();
			if(v.Length == 1)
				glTexCoord1sv(v);
			else if(v.Length == 2)
				glTexCoord2sv(v);
			else if(v.Length == 3)
				glTexCoord3sv(v);
			else if(v.Length == 4)
				glTexCoord4sv(v);
			PostGLCall();
		}

		/// <summary>
		/// This function sets the current texture coordinates.
		/// </summary>
		/// <param name="s">Texture Coordinate.</param>
		/// <param name="t">Texture Coordinate.</param>
		 public static void TexCoord(double s, double t)
		{
			PreGLCall();
			glTexCoord2d(s, t);
			PostGLCall();
		}

		/// <summary>
		/// This function sets the current texture coordinates.
		/// </summary>
		/// <param name="s">Texture Coordinate.</param>
		/// <param name="t">Texture Coordinate.</param>
		 public static void TexCoord(float s, float t)
		{
			PreGLCall();
			glTexCoord2f(s, t);
			PostGLCall();
		}

		/// <summary>
		/// This function sets the current texture coordinates.
		/// </summary>
		/// <param name="s">Texture Coordinate.</param>
		/// <param name="t">Texture Coordinate.</param>
		 public static void TexCoord(int s, int t)
		{
			PreGLCall();
			glTexCoord2i(s, t);
			PostGLCall();
		}

		/// <summary>
		/// This function sets the current texture coordinates.
		/// </summary>
		/// <param name="s">Texture Coordinate.</param>
		/// <param name="t">Texture Coordinate.</param>
		 public static void TexCoord(short s, short t)
		{
			PreGLCall();
			glTexCoord2s(s, t);
			PostGLCall();
		}

        /// <summary>
        /// This function sets the current texture coordinates.
        /// </summary>
        /// <param name="s">Texture Coordinate.</param>
        /// <param name="t">Texture Coordinate.</param>
        /// <param name="r">Texture Coordinate.</param>
         public static void TexCoord(double s, double t, double r)
        {
            PreGLCall();
            glTexCoord3d(s, t, r);
            PostGLCall();
        }

        /// <summary>
        /// This function sets the current texture coordinates.
        /// </summary>
        /// <param name="s">Texture Coordinate.</param>
        /// <param name="t">Texture Coordinate.</param>
        /// <param name="r">Texture Coordinate.</param>
         public static void TexCoord(float s, float t, float r)
        {
            PreGLCall();
            glTexCoord3f(s, t, r);
            PostGLCall();
        }

        /// <summary>
        /// This function sets the current texture coordinates.
        /// </summary>
        /// <param name="s">Texture Coordinate.</param>
        /// <param name="t">Texture Coordinate.</param>
        /// <param name="r">Texture Coordinate.</param>
         public static void TexCoord(int s, int t, int r)
        {
            PreGLCall();
            glTexCoord3i(s, t, r);
            PostGLCall();
        }

        /// <summary>
        /// This function sets the current texture coordinates.
        /// </summary>
        /// <param name="s">Texture Coordinate.</param>
        /// <param name="t">Texture Coordinate.</param>
        /// <param name="r">Texture Coordinate.</param>
         public static void TexCoord(short s, short t, short r)
        {
            PreGLCall();
            glTexCoord3s(s, t, r);
            PostGLCall();
        }

        /// <summary>
        /// This function sets the current texture coordinates.
        /// </summary>
        /// <param name="s">Texture Coordinate.</param>
        /// <param name="t">Texture Coordinate.</param>
        /// <param name="r">Texture Coordinate.</param>
        /// <param name="q">Texture Coordinate.</param>
         public static void TexCoord(double s, double t, double r, double q)
        {
            PreGLCall();
            glTexCoord4d(s, t, r, q);
            PostGLCall();
        }

        /// <summary>
        /// This function sets the current texture coordinates.
        /// </summary>
        /// <param name="s">Texture Coordinate.</param>
        /// <param name="t">Texture Coordinate.</param>
        /// <param name="r">Texture Coordinate.</param>
        /// <param name="q">Texture Coordinate.</param>
         public static void TexCoord(float s, float t, float r, float q)
        {
            PreGLCall();
            glTexCoord4f(s, t, r, q);
            PostGLCall();
        }

        /// <summary>
        /// This function sets the current texture coordinates.
        /// </summary>
        /// <param name="s">Texture Coordinate.</param>
        /// <param name="t">Texture Coordinate.</param>
        /// <param name="r">Texture Coordinate.</param>
        /// <param name="q">Texture Coordinate.</param>
         public static void TexCoord(int s, int t, int r, int q)
        {
            PreGLCall();
            glTexCoord4i(s, t, r, q);
            PostGLCall();
        }

        /// <summary>
        /// This function sets the current texture coordinates.
        /// </summary>
        /// <param name="s">Texture Coordinate.</param>
        /// <param name="t">Texture Coordinate.</param>
        /// <param name="r">Texture Coordinate.</param>
        /// <param name="q">Texture Coordinate.</param>
         public static void TexCoord(short s, short t, short r, short q)
        {
            PreGLCall();
            glTexCoord4s(s, t, r, q);
            PostGLCall();
        }

        /// <summary>
        /// This function sets the texture coord array.
        /// </summary>
        /// <param name="size">The number of coords per set.</param>
        /// <param name="type">The type of data.</param>
        /// <param name="stride">The number of bytes between coords.</param>
        /// <param name="pointer">The coords.</param>
         public static void TexCoordPointer(int size, uint type, int stride, IntPtr pointer)
        {
            PreGLCall();
            glTexCoordPointer(size, type, stride, pointer);
            PostGLCall();
        }

		/// <summary>
		/// This function sets the texture coord array.
		/// </summary>
		/// <param name="size">The number of coords per set.</param>
		/// <param name="type">The type of data.</param>
		/// <param name="stride">The number of bytes between coords.</param>
		/// <param name="pointer">The coords.</param>
		 public static void TexCoordPointer(int size, uint type, int stride, float[] pointer)
		{
			PreGLCall();
			glTexCoordPointer(size, type, stride, pointer);
			PostGLCall();
		}

        /// <summary>
        /// Set texture environment parameters.
        /// </summary>
        /// <param name="target">Specifies a texture environment. Must be OpenGL.TEXTURE_ENV.</param>
        /// <param name="pname">Specifies the symbolic name of a single-valued texture environment parameter. Must be OpenGL.TEXTURE_ENV_MODE.</param>
        /// <param name="param">Specifies a single symbolic constant, one of OpenGL.MODULATE, OpenGL.DECAL, OpenGL.BLEND, or OpenGL.REPLACE.</param>
         public static void TexEnv(uint target, uint pname, float param)
        {
            PreGLCall();
            glTexEnvf(target, pname, param);
            PostGLCall();
        }

        /// <summary>
        /// Set texture environment parameters.
        /// </summary>
        /// <param name="target">Specifies a texture environment. Must be OpenGL.TEXTURE_ENV.</param>
        /// <param name="pname">Specifies the symbolic name of a texture environment parameter. Accepted values are OpenGL.TEXTURE_ENV_MODE and OpenGL.TEXTURE_ENV_COLOR.</param>
        /// <param name="parameters">Specifies a pointer to a parameter array that contains either a single symbolic constant or an RGBA color.</param>
         public static void TexEnv(uint target, uint pname, float[] parameters)
        {
            PreGLCall();
            glTexEnvfv(target, pname, parameters);
            PostGLCall();
        }

        /// <summary>
        /// Set texture environment parameters.
        /// </summary>
        /// <param name="target">Specifies a texture environment. Must be OpenGL.TEXTURE_ENV.</param>
        /// <param name="pname">Specifies the symbolic name of a single-valued texture environment parameter. Must be OpenGL.TEXTURE_ENV_MODE.</param>
        /// <param name="param">Specifies a single symbolic constant, one of OpenGL.MODULATE, OpenGL.DECAL, OpenGL.BLEND, or OpenGL.REPLACE.</param>
         public static void TexEnv(uint target, uint pname, int param)
        {
            PreGLCall();
            glTexEnvi(target, pname, param);
            PostGLCall();
        }

        /// <summary>
        /// Set texture environment parameters.
        /// </summary>
        /// <param name="target">Specifies a texture environment. Must be OpenGL.TEXTURE_ENV.</param>
        /// <param name="pname">Specifies the symbolic name of a texture environment parameter. Accepted values are OpenGL.TEXTURE_ENV_MODE and OpenGL.TEXTURE_ENV_COLOR.</param>
        /// <param name="parameters">Specifies a pointer to a parameter array that contains either a single symbolic constant or an RGBA color.</param>
         public static void TexEnv(uint target, uint pname, int[] parameters)
        {
            PreGLCall();
            glTexGeniv(target, pname, parameters);
            PostGLCall();
        }

        /// <summary>
        /// Control the generation of texture coordinates.
        /// </summary>
        /// <param name="coord">Specifies a texture coordinate. Must be one of OpenGL.S, OpenGL.T, OpenGL.R, or OpenGL.Q.</param>
        /// <param name="pname">Specifies the symbolic name of the texture-coordinate generation function. Must be OpenGL.TEXTURE_GEN_MODE.</param>
        /// <param name="param">Specifies a single-valued texture generation parameter, one of OpenGL.OBJECT_LINEAR, OpenGL.GL_EYE_LINEAR, or OpenGL.SPHERE_MAP.</param>
		 public static void TexGen(uint coord, uint pname, double param)
        {
            PreGLCall();
            glTexGend(coord, pname, param);
            PostGLCall();
        }

        /// <summary>
        /// Control the generation of texture coordinates.
        /// </summary>
        /// <param name="coord">Specifies a texture coordinate. Must be one of OpenGL.S, OpenGL.T, OpenGL.R, or OpenGL.Q.</param>
        /// <param name="pname">Specifies the symbolic name of the texture-coordinate generation function or function parameters. Must be OpenGL.TEXTURE_GEN_MODE, OpenGL.OBJECT_PLANE, or OpenGL.EYE_PLANE.</param>
        /// <param name="parameters">Specifies a pointer to an array of texture generation parameters. If pname is OpenGL.TEXTURE_GEN_MODE, then the array must contain a single symbolic constant, one of OpenGL.OBJECT_LINEAR, OpenGL.EYE_LINEAR, or OpenGL.SPHERE_MAP. Otherwise, params holds the coefficients for the texture-coordinate generation function specified by pname.</param>
         public static void TexGen(uint coord, uint pname, double[] parameters) 
        {
            PreGLCall();
            glTexGendv(coord, pname, parameters);
            PostGLCall();
        }

        /// <summary>
        /// Control the generation of texture coordinates.
        /// </summary>
        /// <param name="coord">Specifies a texture coordinate. Must be one of OpenGL.S, OpenGL.T, OpenGL.R, or OpenGL.Q.</param>
        /// <param name="pname">Specifies the symbolic name of the texture-coordinate generation function. Must be OpenGL.TEXTURE_GEN_MODE.</param>
        /// <param name="param">Specifies a single-valued texture generation parameter, one of OpenGL.OBJECT_LINEAR, OpenGL.GL_EYE_LINEAR, or OpenGL.SPHERE_MAP.</param>
         public static void TexGen(uint coord, uint pname, float param)
        {
            PreGLCall();
            glTexGenf(coord, pname, param);
            PostGLCall();
        }

        /// <summary>
        /// Control the generation of texture coordinates.
        /// </summary>
        /// <param name="coord">Specifies a texture coordinate. Must be one of OpenGL.S, OpenGL.T, OpenGL.R, or OpenGL.Q.</param>
        /// <param name="pname">Specifies the symbolic name of the texture-coordinate generation function or function parameters. Must be OpenGL.TEXTURE_GEN_MODE, OpenGL.OBJECT_PLANE, or OpenGL.EYE_PLANE.</param>
        /// <param name="parameters">Specifies a pointer to an array of texture generation parameters. If pname is OpenGL.TEXTURE_GEN_MODE, then the array must contain a single symbolic constant, one of OpenGL.OBJECT_LINEAR, OpenGL.EYE_LINEAR, or OpenGL.SPHERE_MAP. Otherwise, params holds the coefficients for the texture-coordinate generation function specified by pname.</param>
         public static void TexGen(uint coord, uint pname, float[] parameters)
        {
            PreGLCall();
            glTexGenfv(coord, pname, parameters);
            PostGLCall();
        }

        /// <summary>
        /// Control the generation of texture coordinates.
        /// </summary>
        /// <param name="coord">Specifies a texture coordinate. Must be one of OpenGL.S, OpenGL.T, OpenGL.R, or OpenGL.Q.</param>
        /// <param name="pname">Specifies the symbolic name of the texture-coordinate generation function. Must be OpenGL.TEXTURE_GEN_MODE.</param>
        /// <param name="param">Specifies a single-valued texture generation parameter, one of OpenGL.OBJECT_LINEAR, OpenGL.GL_EYE_LINEAR, or OpenGL.SPHERE_MAP.</param>
         public static void TexGen(uint coord, uint pname, int param)
        {
            PreGLCall();
            glTexGeni(coord, pname, param);
            PostGLCall();
        }

        /// <summary>
        /// Control the generation of texture coordinates.
        /// </summary>
        /// <param name="coord">Specifies a texture coordinate. Must be one of OpenGL.S, OpenGL.T, OpenGL.R, or OpenGL.Q.</param>
        /// <param name="pname">Specifies the symbolic name of the texture-coordinate generation function or function parameters. Must be OpenGL.TEXTURE_GEN_MODE, OpenGL.OBJECT_PLANE, or OpenGL.EYE_PLANE.</param>
        /// <param name="parameters">Specifies a pointer to an array of texture generation parameters. If pname is OpenGL.TEXTURE_GEN_MODE, then the array must contain a single symbolic constant, one of OpenGL.OBJECT_LINEAR, OpenGL.EYE_LINEAR, or OpenGL.SPHERE_MAP. Otherwise, params holds the coefficients for the texture-coordinate generation function specified by pname.</param>
         public static void TexGen(uint coord, uint pname, int[] parameters)
        {
            PreGLCall();
            glTexGeniv(coord, pname, parameters);
            PostGLCall();
        }

		/// <summary>
		/// This function sets the image for the currently binded texture.
		/// </summary>
		/// <param name="target">The type of texture, TEXTURE_2D or PROXY_TEXTURE_2D.</param>
		/// <param name="level">For mip-map textures, ordinary textures should be '0'.</param>
		/// <param name="internalformat">The format of the data you are want OpenGL to create, e.g  RGB16.</param>
		/// <param name="width">The width of the texture image (must be a power of 2, e.g 64).</param>
		/// <param name="border">The width of the border (0 or 1).</param>
		/// <param name="format">The format of the data you are passing, e.g. RGBA.</param>
		/// <param name="type">The type of data you are passing, e.g GL_BYTE.</param>
		/// <param name="pixels">The actual pixel data.</param>
		 public static void TexImage1D(uint target, int level, uint internalformat, int width, int border, uint format, uint type,  byte[] pixels)
		{
			PreGLCall();
			glTexImage1D(target, level, internalformat, width, border, format, type, pixels);
			PostGLCall();
		}

		/// <summary>
		/// This function sets the image for the currently binded texture.
		/// </summary>
		/// <param name="target">The type of texture, TEXTURE_2D or PROXY_TEXTURE_2D.</param>
		/// <param name="level">For mip-map textures, ordinary textures should be '0'.</param>
		/// <param name="internalformat">The format of the data you are want OpenGL to create, e.g  RGB16.</param>
		/// <param name="width">The width of the texture image (must be a power of 2, e.g 64).</param>
		/// <param name="height">The height of the texture image (must be a power of 2, e.g 32).</param>
		/// <param name="border">The width of the border (0 or 1).</param>
		/// <param name="format">The format of the data you are passing, e.g. RGBA.</param>
		/// <param name="type">The type of data you are passing, e.g GL_BYTE.</param>
		/// <param name="pixels">The actual pixel data.</param>
		 public static void TexImage2D(uint target, int level, uint internalformat, int width, int height, int border, uint format, uint type, byte[] pixels)
		{
			PreGLCall();
			glTexImage2D(target, level, internalformat, width, height, border, format, type, pixels);
			PostGLCall();
		}

        /// <summary>
        /// This function sets the image for the currently binded texture.
        /// </summary>
        /// <param name="target">The type of texture, TEXTURE_2D or PROXY_TEXTURE_2D.</param>
        /// <param name="level">For mip-map textures, ordinary textures should be '0'.</param>
        /// <param name="internalformat">The format of the data you are want OpenGL to create, e.g  RGB16.</param>
        /// <param name="width">The width of the texture image (must be a power of 2, e.g 64).</param>
        /// <param name="height">The height of the texture image (must be a power of 2, e.g 32).</param>
        /// <param name="border">The width of the border (0 or 1).</param>
        /// <param name="format">The format of the data you are passing, e.g. RGBA.</param>
        /// <param name="type">The type of data you are passing, e.g GL_BYTE.</param>
        /// <param name="pixels">The actual pixel data.</param>
         public static void TexImage2D(uint target, int level, uint internalformat, int width, int height, int border, uint format, uint type, IntPtr pixels)
        {
            PreGLCall();
            glTexImage2D(target, level, internalformat, width, height, border, format, type, pixels);
            PostGLCall();
        }

		/// <summary>
		///	This function sets the parameters for the currently binded texture object.
		/// </summary>
		/// <param name="target">The type of texture you are setting the parameter to, e.g. TEXTURE_2D</param>
		/// <param name="pname">The parameter to set.</param>
		/// <param name="param">The value to set it to.</param>
		 public static void TexParameter(uint target, uint pname, float param)
		{
			PreGLCall();
			glTexParameterf(target, pname, param);
			PostGLCall();
		}

        /// <summary>
        ///	This function sets the parameters for the currently binded texture object.
        /// </summary>
        /// <param name="target">The type of texture you are setting the parameter to, e.g. TEXTURE_2D</param>
        /// <param name="pname">The parameter to set.</param>
        /// <param name="param">The value to set it to.</param>
         public static void TexParameter(TextureTarget target, TextureParameter pname, float param)
        {
            PreGLCall();
            glTexParameterf((uint)target, (uint)pname, param);
            PostGLCall();
        }

		/// <summary>
		///	This function sets the parameters for the currently binded texture object.
		/// </summary>
		/// <param name="target">The type of texture you are setting the parameter to, e.g. TEXTURE_2D</param>
		/// <param name="pname">The parameter to set.</param>
		/// <param name="parameters">The value to set it to.</param>
		 public static void TexParameter(uint target, uint pname, float[] parameters)
		{
			PreGLCall();
			glTexParameterfv(target, pname, parameters);
			PostGLCall();
		}

        /// <summary>
        ///	This function sets the parameters for the currently binded texture object.
        /// </summary>
        /// <param name="target">The type of texture you are setting the parameter to, e.g. TEXTURE_2D</param>
        /// <param name="pname">The parameter to set.</param>
        /// <param name="parameters">The value to set it to.</param>
         public static void TexParameter(TextureTarget target, TextureParameter pname, float[] parameters)
        {
            PreGLCall();
            glTexParameterfv((uint)target, (uint)pname, parameters);
            PostGLCall();
        }

		/// <summary>
		///	This function sets the parameters for the currently binded texture object.
		/// </summary>
		/// <param name="target">The type of texture you are setting the parameter to, e.g. TEXTURE_2D</param>
		/// <param name="pname">The parameter to set.</param>
		/// <param name="param">The value to set it to.</param>
		 public static void TexParameter(uint target, uint pname, int param)
		{
			PreGLCall();
			glTexParameteri(target, pname, param);
			PostGLCall();
		}

        /// <summary>
        ///	This function sets the parameters for the currently binded texture object.
        /// </summary>
        /// <param name="target">The type of texture you are setting the parameter to, e.g. TEXTURE_2D</param>
        /// <param name="pname">The parameter to set.</param>
        /// <param name="param">The value to set it to.</param>
         public static void TexParameter(TextureTarget target, TextureParameter pname, int param)
        {
            PreGLCall();
            glTexParameteri((uint)target, (uint)pname, param);
            PostGLCall();
        }

		/// <summary>
		///	This function sets the parameters for the currently binded texture object.
		/// </summary>
		/// <param name="target">The type of texture you are setting the parameter to, e.g. TEXTURE_2D</param>
		/// <param name="pname">The parameter to set.</param>
		/// <param name="parameters">The value to set it to.</param>
		 public static void TexParameter(uint target, uint pname, int[] parameters)
		{
			PreGLCall();
			glTexParameteriv(target, pname, parameters);
			PostGLCall();
		}

        /// <summary>
        ///	This function sets the parameters for the currently binded texture object.
        /// </summary>
        /// <param name="target">The type of texture you are setting the parameter to, e.g. TEXTURE_2D</param>
        /// <param name="pname">The parameter to set.</param>
        /// <param name="parameters">The value to set it to.</param>
         public static void TexParameter(TextureTarget target, TextureParameter pname, int[] parameters)
        {
            PreGLCall();
            glTexParameteriv((uint)target, (uint)pname, parameters);
            PostGLCall();
        }

        /// <summary>
        /// Specify a two-dimensional texture subimage.
        /// </summary>
        /// <param name="target">Specifies the target texture. Must be OpenGL.TEXTURE_1D.</param>
        /// <param name="level">Specifies the level-of-detail number. Level 0 is the base image level. Level n is the nth mipmap reduction image.</param>
        /// <param name="xoffset">Specifies a texel offset in the x direction within the texture array.</param>
        /// <param name="width">Specifies the width of the texture subimage.</param>
        /// <param name="format">Specifies the format of the pixel data.</param>
        /// <param name="type">Specifies the data type of the pixel	data.</param>
        /// <param name="pixels">Specifies a pointer to the image data in memory.</param>
         public static void TexSubImage1D(uint target, int level, int xoffset, int width, uint format, uint type, int[] pixels)
        {
            PreGLCall();
            glTexSubImage1D(target, level, xoffset, width, format, type, pixels);
            PostGLCall();
        }

        /// <summary>
        /// Specify a two-dimensional texture subimage.
        /// </summary>
        /// <param name="target">Specifies the target texture. Must be OpenGL.TEXTURE_1D.</param>
        /// <param name="level">Specifies the level-of-detail number. Level 0 is the base image level. Level n is the nth mipmap reduction image.</param>
        /// <param name="xoffset">Specifies a texel offset in the x direction within the texture array.</param>
        /// <param name="yoffset">Specifies a texel offset in the y direction within the texture array.</param>
        /// <param name="width">Specifies the width of the texture subimage.</param>
        /// <param name="height">Specifies the height of the texture subimage.</param>
        /// <param name="format">Specifies the format of the pixel data.</param>
        /// <param name="type">Specifies the data type of the pixel	data.</param>
        /// <param name="pixels">Specifies a pointer to the image data in memory.</param>
         public static void TexSubImage2D(uint target, int level, int xoffset, int yoffset, int width, int height, uint format, uint type, int[] pixels)
        {
            PreGLCall();
            glTexSubImage2D(target, level, xoffset, yoffset, width, height, format, type, pixels);
            PostGLCall();
        }

		/// <summary>
		/// This function applies a translation transformation to the current matrix.
		/// </summary>
		/// <param name="x">The amount to translate along the x axis.</param>
		/// <param name="y">The amount to translate along the y axis.</param>
		/// <param name="z">The amount to translate along the z axis.</param>
		 public static void Translate(double x, double y, double z)
		{
			PreGLCall();
			glTranslated(x, y, z);
			PostGLCall();
		}

		/// <summary>
		/// This function applies a translation transformation to the current matrix.
		/// </summary>
		/// <param name="x">The amount to translate along the x axis.</param>
		/// <param name="y">The amount to translate along the y axis.</param>
		/// <param name="z">The amount to translate along the z axis.</param>
		 public static void Translate(float x, float y, float z)
		{
			PreGLCall();
			glTranslatef(x, y, z);
			PostGLCall();
		}

		/// <summary>
		/// This function turns a screen Coordinate into a world coordinate.
		/// </summary>
		/// <param name="winx">Screen Coordinate.</param>
		/// <param name="winy">Screen Coordinate.</param>
		/// <param name="winz">Screen Coordinate.</param>
		/// <param name="modelMatrix">Current ModelView matrix.</param>
		/// <param name="projMatrix">Current Projection matrix.</param>
		/// <param name="viewport">Current Viewport.</param>
		/// <param name="objx">The world coordinate.</param>
		/// <param name="objy">The world coordinate.</param>
		/// <param name="objz">The world coordinate.</param>
		 public static void UnProject(double winx, double winy, double winz, 
			double[] modelMatrix, double[] projMatrix, int[] viewport, 
			ref double objx, ref double objy, ref double objz)
		{
			PreGLCall();
			gluUnProject(winx, winy, winz, modelMatrix, projMatrix, viewport,
				ref objx, ref objy, ref objz);
			PostGLCall();
		}

		/// <summary>
		/// This is a convenience function. It calls UnProject with the current 
		/// viewport, modelview and persective matricies, saving you from getting them.
		/// To use you own matricies, all the other version of UnProject.
		/// </summary>
		/// <param name="winx">X Coordinate (Screen Coordinate).</param>
		/// <param name="winy">Y Coordinate (Screen Coordinate).</param>
		/// <param name="winz">Z Coordinate (Screen Coordinate).</param>
		/// <returns>The world coordinate.</returns>
		public static double[] UnProject(double winx, double winy, double winz)
		{
			PreGLCall();

			var modelview = new double[16];
			var projection = new double[16];
			var viewport = new int[4];
            GetDouble(GL_MODELVIEW_MATRIX, modelview);
            GetDouble(GL_PROJECTION_MATRIX, projection);
            GetInteger(GL_VIEWPORT, viewport);
            var result = new double[3];
            gluUnProject(winx, winy, winz, modelview, projection, viewport, ref result[0], ref result[1], ref result[2]);

			PostGLCall();

            return result;
		}

		/// <summary>
		/// Set the current vertex (must be called between 'Begin' and 'End').
		/// </summary>
		/// <param name="x">X Value.</param>
		/// <param name="y">Y Value.</param>
		 public static void Vertex(double x, double y)
		{
			PreGLCall();
			glVertex2d(x, y);
			PostGLCall();
		}

        /// <summary>
        /// Set the current vertex (must be called between 'Begin' and 'End').
        /// </summary>
        /// <param name="v">Specifies the coordinate.</param>
         public static void Vertex(double[] v)
        {
            PreGLCall();
            if (v.Length == 2)
                glVertex2dv(v);
            else if (v.Length == 3)
                glVertex3dv(v);
            else if (v.Length == 4)
                glVertex4dv(v);
            PostGLCall();
        }

        /// <summary>
        /// Set the current vertex (must be called between 'Begin' and 'End').
        /// </summary>
        /// <param name="x">X Value.</param>
        /// <param name="y">Y Value.</param>
         public static void Vertex(float x, float y)
        {
            PreGLCall();
            glVertex2f(x, y);
            PostGLCall();
        }

        /// <summary>
        /// Set the current vertex (must be called between 'Begin' and 'End').
        /// </summary>
        /// <param name="x">X Value.</param>
        /// <param name="y">Y Value.</param>
         public static void Vertex(int x, int y)
        {
            PreGLCall();
            glVertex2i(x, y);
            PostGLCall();
        }

        /// <summary>
        /// Set the current vertex (must be called between 'Begin' and 'End').
        /// </summary>
        /// <param name="v">Specifies the coordinate.</param>
         public static void Vertex(int[] v)
        {
            PreGLCall();
            if (v.Length == 2)
                glVertex2iv(v);
            else if (v.Length == 3)
                glVertex3iv(v);
            else if (v.Length == 4)
                glVertex4iv(v);
            PostGLCall();
        }

        /// <summary>
        /// Set the current vertex (must be called between 'Begin' and 'End').
        /// </summary>
        /// <param name="x">X Value.</param>
        /// <param name="y">Y Value.</param>
         public static void Vertex(short x, short y)
        {
            PreGLCall();
            glVertex2s(x, y);
            PostGLCall();
        }

        /// <summary>
        /// Set the current vertex (must be called between 'Begin' and 'End').
        /// </summary>
        /// <param name="v">Specifies the coordinate.</param>
         public static void Vertex2sv(short[] v)
        {
            PreGLCall();
            if (v.Length == 2)
                glVertex2sv(v);
            else if (v.Length == 3)
                glVertex3sv(v);
            else if (v.Length == 4)
                glVertex4sv(v);
            PostGLCall();
        }

		/// <summary>
		/// Set the current vertex (must be called between 'Begin' and 'End').
		/// </summary>
		/// <param name="x">X Value.</param>
		/// <param name="y">Y Value.</param>
		/// <param name="z">Z Value.</param>
		 public static void Vertex(double x, double y, double z)
		{
			PreGLCall();
			glVertex3d(x, y, z);
			PostGLCall();
		}

		/// <summary>
		/// Set the current vertex (must be called between 'Begin' and 'End').
		/// </summary>
		/// <param name="x">X Value.</param>
		/// <param name="y">Y Value.</param>
		/// <param name="z">Z Value.</param>
		 public static void Vertex(float x, float y, float z)
		{
			PreGLCall();
			glVertex3f(x, y, z);
			PostGLCall();
		}

		/// <summary>
		/// Sets the current vertex (must be called between 'Begin' and 'End').
		/// </summary>
		/// <param name="v">An array of 2, 3 or 4 floats.</param>
		 public static void Vertex(float []v)
		{
			PreGLCall();
			if(v.Length == 2)
				glVertex2fv(v);
			else if(v.Length == 3)
				glVertex3fv(v);
			else if(v.Length == 4)
				glVertex4fv(v);
			PostGLCall();
		}

        /// <summary>
        /// Set the current vertex (must be called between 'Begin' and 'End').
        /// </summary>
        /// <param name="x">X Value.</param>
        /// <param name="y">Y Value.</param>
        /// <param name="z">Z Value.</param>
         public static void Vertex(int x, int y, int z)
        {
            PreGLCall();
            glVertex3i(x, y, z);
            PostGLCall();
        }

        /// <summary>
        /// Set the current vertex (must be called between 'Begin' and 'End').
        /// </summary>
        /// <param name="x">X Value.</param>
        /// <param name="y">Y Value.</param>
        /// <param name="z">Z Value.</param>
         public static void Vertex(short x, short y, short z)
        {
            PreGLCall();
            glVertex3s(x, y, z);
            PostGLCall();
        }

        /// <summary>
        /// Set the current vertex (must be called between 'Begin' and 'End').
        /// </summary>
        /// <param name="x">X Value.</param>
        /// <param name="y">Y Value.</param>
        /// <param name="z">Z Value.</param>
        /// <param name="w">W Value.</param>
         public static void Vertex4d(double x, double y, double z, double w)
        {
            PreGLCall();
            glVertex4d(x, y, z, w);
            PostGLCall();
        }

        /// <summary>
        /// Set the current vertex (must be called between 'Begin' and 'End').
        /// </summary>
        /// <param name="x">X Value.</param>
        /// <param name="y">Y Value.</param>
        /// <param name="z">Z Value.</param>
        /// <param name="w">W Value.</param>
         public static void Vertex4f(float x, float y, float z, float w)
        {
            PreGLCall();
            glVertex4f(x, y, z, w);
            PostGLCall();
        }

        /// <summary>
        /// Set the current vertex (must be called between 'Begin' and 'End').
        /// </summary>
        /// <param name="x">X Value.</param>
        /// <param name="y">Y Value.</param>
        /// <param name="z">Z Value.</param>
        /// <param name="w">W Value.</param>
         public static void Vertex4i(int x, int y, int z, int w)
        {
            PreGLCall();
            glVertex4i(x, y, z, w);
            PostGLCall();
        }

        /// <summary>
        /// Set the current vertex (must be called between 'Begin' and 'End').
        /// </summary>
        /// <param name="x">X Value.</param>
        /// <param name="y">Y Value.</param>
        /// <param name="z">Z Value.</param>
        /// <param name="w">W Value.</param>
         public static void Vertex4s(short x, short y, short z, short w)
        {
            PreGLCall();
            glVertex4s(x, y, z, w);
            PostGLCall();
        }

		/// <summary>
		/// This function sets the address of the vertex pointer array.
		/// </summary>
		/// <param name="size">The number of coords per vertex.</param>
		/// <param name="type">The data type.</param>
		/// <param name="stride">The byte offset between vertices.</param>
		/// <param name="pointer">The array.</param>
		 public static void VertexPointer(int size, uint type, int stride, IntPtr pointer)
		{
			PreGLCall();
			glVertexPointer(size, type, stride, pointer);
			PostGLCall();
		}

        /// <summary>
        /// This function sets the address of the vertex pointer array.
        /// </summary>
        /// <param name="size">The number of coords per vertex.</param>
        /// <param name="stride">The byte offset between vertices.</param>
        /// <param name="pointer">The array.</param>
         public static void VertexPointer(int size, int stride, short[] pointer)
        {
            PreGLCall();
            glVertexPointer(size, GL_SHORT, stride, pointer);
            PostGLCall();
        }

        /// <summary>
        /// This function sets the address of the vertex pointer array.
        /// </summary>
        /// <param name="size">The number of coords per vertex.</param>
        /// <param name="stride">The byte offset between vertices.</param>
        /// <param name="pointer">The array.</param>
         public static void VertexPointer(int size, int stride, int[] pointer)
        {
            PreGLCall();
            glVertexPointer(size, GL_INT, stride, pointer);
            PostGLCall();
        }

        /// <summary>
        /// This function sets the address of the vertex pointer array.
        /// </summary>
        /// <param name="size">The number of coords per vertex.</param>
        /// <param name="stride">The byte offset between vertices.</param>
        /// <param name="pointer">The array.</param>
         public static void VertexPointer(int size, int stride, float[] pointer)
        {
            PreGLCall();
            glVertexPointer(size, GL_FLOAT, stride, pointer);
            PostGLCall();
        }

        /// <summary>
        /// This function sets the address of the vertex pointer array.
        /// </summary>
        /// <param name="size">The number of coords per vertex.</param>
        /// <param name="stride">The byte offset between vertices.</param>
        /// <param name="pointer">The array.</param>
         public static void VertexPointer(int size, int stride, double[] pointer)
        {
            PreGLCall();
            glVertexPointer(size, GL_DOUBLE, stride, pointer);
            PostGLCall();
        }

		/// <summary>
		/// This sets the viewport of the current Render Context. Normally x and y are 0
		/// and the width and height are just those of the control/graphics you are drawing
		/// to.
		/// </summary>
		/// <param name="x">Top-Left point of the viewport.</param>
		/// <param name="y">Top-Left point of the viewport.</param>
		/// <param name="width">Width of the viewport.</param>
		/// <param name="height">Height of the viewport.</param>
		 public static void Viewport (int x, int y, int width, int height)
		{
			PreGLCall();
			glViewport(x, y, width, height);
			PostGLCall();
		}
		
        /// <summary>
        /// Produce an error string from a GL or GLU error code.
        /// </summary>
        /// <param name="errCode">Specifies a GL or GLU error code.</param>
        /// <returns>The OpenGL/GLU error string.</returns>
		public static unsafe string ErrorString(uint errCode)
        {
            PreGLCall();
            sbyte* pStr = gluErrorString(errCode);
            var str = new string(pStr);
            PostGLCall();

            return str;
        }

        /// <summary>
        /// Return a string describing the GLU version or GLU extensions.
        /// </summary>
        /// <param name="name">Specifies a symbolic constant, one of OpenGL.VERSION, or OpenGL.EXTENSIONS.</param>
        /// <returns>The GLU string.</returns>
		public static unsafe string GetString(int name)
        {
            PreGLCall();
            sbyte* pStr = gluGetString(name);
            var str = new string(pStr);
            PostGLCall();

            return str;
        }

        /// <summary>
        /// Scale an image to an arbitrary size.
        /// </summary>
        /// <param name="format">Specifies the format of the pixel data.</param>
        /// <param name="widthin">Specify the width of the source image	that is	scaled.</param>
        /// <param name="heightin">Specify the height of the source image that is scaled.</param>
        /// <param name="typein">Specifies the data type for dataIn.</param>
        /// <param name="datain">Specifies a pointer to the source image.</param>
        /// <param name="widthout">Specify the width of the destination image.</param>
        /// <param name="heightout">Specify the height of the destination image.</param>
        /// <param name="typeout">Specifies the data type for dataOut.</param>
        /// <param name="dataout">Specifies a pointer to the destination image.</param>
		 public static void ScaleImage(int format, int widthin, int heightin, int typein, int[] datain, int widthout, int heightout, int typeout, int[] dataout)
        {
            PreGLCall();
            gluScaleImage(format, widthin, heightin, typein, datain, widthout, heightout, typeout, dataout);
            PostGLCall();
        }

        /// <summary>
        /// Create 1-D mipmaps.
        /// </summary>
        /// <param name="target">Specifies the target texture. Must be OpenGL.TEXTURE_1D.</param>
        /// <param name="components">Specifies the number of color components in the texture. Must be 1, 2, 3, or 4.</param>
        /// <param name="width">Specifies the width of the texture image.</param>
        /// <param name="format">Specifies the format of the pixel data.</param>
        /// <param name="type">Specifies the data type for data.</param>
        /// <param name="data">Specifies a pointer to the image data in memory.</param>
         public static void Build1DMipmaps(uint target, uint components, int width, uint format, uint type, IntPtr data)
        {
            PreGLCall();
            gluBuild1DMipmaps(target, components, width, format, type, data);
            PostGLCall();
        }

        /// <summary>
        /// Create 2-D mipmaps.
        /// </summary>
        /// <param name="target">Specifies the target texture. Must be OpenGL.TEXTURE_1D.</param>
        /// <param name="components">Specifies the number of color components in the texture. Must be 1, 2, 3, or 4.</param>
        /// <param name="width">Specifies the width of the texture image.</param>
        /// <param name="height">Specifies the height of the texture image.</param>
        /// <param name="format">Specifies the format of the pixel data.</param>
        /// <param name="type">Specifies the data type for data.</param>
        /// <param name="data">Specifies a pointer to the image data in memory.</param>
		 public static void Build2DMipmaps(uint target, uint components, int width, int height, uint format, uint type, IntPtr data)
        {
            PreGLCall();
            gluBuild2DMipmaps(target, components, width, height, format, type, data);
            PostGLCall();
        }

        /// <summary>
        /// Draw a disk.
        /// </summary>
        /// <param name="qobj">Specifies the quadrics object (created with gluNewQuadric).</param>
        /// <param name="innerRadius">Specifies the	inner radius of	the disk (may be 0).</param>
        /// <param name="outerRadius">Specifies the	outer radius of	the disk.</param>
        /// <param name="slices">Specifies the	number of subdivisions around the z axis.</param>
        /// <param name="loops">Specifies the	number of concentric rings about the origin into which the disk is subdivided.</param>
		 public static void Disk(IntPtr qobj, double innerRadius, double outerRadius, int slices, int loops)
        {
            PreGLCall();
            gluDisk(qobj, innerRadius, outerRadius, slices, loops);
            PostGLCall();
        }

        /// <summary>
        /// Create a tessellation object.
        /// </summary>
        /// <returns>A new GLUtesselator poiner.</returns>
		public static IntPtr NewTess()
        {
            PreGLCall();
            IntPtr returnValue = gluNewTess();
            PostGLCall();

            return returnValue;
        }

        /// <summary>
        /// Delete a tesselator object.
        /// </summary>
        /// <param name="tess">The tesselator pointer.</param>
		 public static void DeleteTess(IntPtr tess)
        {
            PreGLCall();
            gluDeleteTess(tess);
            PostGLCall();
        }

        /// <summary>
        /// Delimit a polygon description.
        /// </summary>
        /// <param name="tess">Specifies the tessellation object (created with gluNewTess).</param>
        /// <param name="polygonData">Specifies a pointer to user polygon data.</param>
		 public static void TessBeginPolygon(IntPtr tess, IntPtr polygonData)
        {
            PreGLCall();
            gluTessBeginPolygon(tess, polygonData);
            PostGLCall();
        }

        /// <summary>
        /// Delimit a contour description.
        /// </summary>
        /// <param name="tess">Specifies the tessellation object (created with gluNewTess).</param>
		 public static void TessBeginContour(IntPtr tess)
        {
            PreGLCall();
            gluTessBeginContour(tess);
        }

        /// <summary>
        /// Specify a vertex on a polygon.
        /// </summary>
        /// <param name="tess">Specifies the tessellation object (created with gluNewTess).</param>
        /// <param name="coords">Specifies the location of the vertex.</param>
        /// <param name="data">Specifies an opaque	pointer	passed back to the program with the vertex callback (as specified by gluTessCallback).</param>
		 public static void TessVertex(IntPtr tess, double[] coords, double[] data)
        {
            PreGLCall();
            gluTessVertex(tess, coords, data);
            PostGLCall();
        }

        /// <summary>
        /// Delimit a contour description.
        /// </summary>
        /// <param name="tess">Specifies the tessellation object (created with gluNewTess).</param>
		 public static void TessEndContour(IntPtr tess)
        {
            PreGLCall();
            gluTessEndContour(tess);
            PostGLCall();
        }

        /// <summary>
        /// Delimit a polygon description.
        /// </summary>
        /// <param name="tess">Specifies the tessellation object (created with gluNewTess).</param>
		 public static void TessEndPolygon(IntPtr tess)
        {
            PreGLCall();
            gluTessEndPolygon(tess);
            PostGLCall();
        }

        /// <summary>
        /// Set a tessellation object property.
        /// </summary>
        /// <param name="tess">Specifies the tessellation object (created with gluNewTess).</param>
        /// <param name="which">Specifies the property to be set.</param>
        /// <param name="value">Specifies the value of	the indicated property.</param>
		 public static void TessProperty(IntPtr tess, int which, double value)
        {
            PreGLCall();
            gluTessProperty(tess, which, value);
            PostGLCall();
        }

        /// <summary>
        /// Specify a normal for a polygon.
        /// </summary>
        /// <param name="tess">Specifies the tessellation object (created with gluNewTess).</param>
        /// <param name="x">Specifies the first component of the normal.</param>
        /// <param name="y">Specifies the second component of the normal.</param>
        /// <param name="z">Specifies the third component of the normal.</param>
		 public static void TessNormal(IntPtr tess, double x, double y, double z)
        {
            PreGLCall();
            gluTessNormal(tess, x, y, z);
            PostGLCall();
        }

        /// <summary>
        /// Set a tessellation object property.
        /// </summary>
        /// <param name="tess">Specifies the tessellation object (created with gluNewTess).</param>
        /// <param name="which">Specifies the property	to be set.</param>
        /// <param name="value">Specifies the value of	the indicated property.</param>
		 public static void GetTessProperty(IntPtr tess, int which, double value)
        {
            PreGLCall();
            gluGetTessProperty(tess, which, value);
            PostGLCall();
        }

        /// <summary>
        /// Delimit a NURBS trimming loop definition.
        /// </summary>
        /// <param name="nobj">Specifies the NURBS object (created with gluNewNurbsRenderer).</param>
		 public static void BeginTrim(IntPtr nobj)
        {
            PreGLCall();
            gluBeginTrim(nobj);
            PostGLCall();
        }

        /// <summary>
        /// Delimit a NURBS trimming loop definition.
        /// </summary>
        /// <param name="nobj">Specifies the NURBS object (created with gluNewNurbsRenderer).</param>
		 public static void EndTrim(IntPtr nobj)
        {
            PreGLCall();
            gluEndTrim(nobj);
            PostGLCall();
        }

        /// <summary>
        /// Describe a piecewise linear NURBS trimming curve.
        /// </summary>
        /// <param name="nobj">Specifies the NURBS object (created with gluNewNurbsRenderer).</param>
        /// <param name="count">Specifies the number of points on the curve.</param>
        /// <param name="array">Specifies an array containing the curve points.</param>
        /// <param name="stride">Specifies the offset (a number of single-precision floating-point values) between points on the curve.</param>
        /// <param name="type">Specifies the type of curve. Must be either OpenGL.MAP1_TRIM_2 or OpenGL.MAP1_TRIM_3.</param>
		 public static void PwlCurve(IntPtr nobj, int count, float array, int stride, uint type)
        {
            PreGLCall();
            gluPwlCurve(nobj, count, array, stride, type);
            PostGLCall();
        }

        /// <summary>
        /// Load NURBS sampling and culling matrice.
        /// </summary>
        /// <param name="nobj">Specifies the NURBS object (created with gluNewNurbsRenderer).</param>
        /// <param name="modelMatrix">Specifies a modelview matrix (as from a glGetFloatv call).</param>
        /// <param name="projMatrix">Specifies a projection matrix (as from a glGetFloatv call).</param>
        /// <param name="viewport">Specifies a viewport (as from a glGetIntegerv call).</param>
		 public static void LoadSamplingMatrices(IntPtr nobj, float[] modelMatrix, float[] projMatrix, int[] viewport)
        {
            PreGLCall();
            gluLoadSamplingMatrices(nobj, modelMatrix, projMatrix, viewport);
            PostGLCall();
        }

        /// <summary>
        /// Get a NURBS property.
        /// </summary>
        /// <param name="nobj">Specifies the NURBS object (created with gluNewNurbsRenderer).</param>
        /// <param name="property">Specifies the property whose value is to be fetched.</param>
        /// <param name="value">Specifies a pointer to the location into which the value of the named property is written.</param>
		 public static void GetNurbsProperty(IntPtr nobj, int property, float value)
        {
            PreGLCall();
            gluGetNurbsProperty(nobj, property, value);
            PostGLCall();
        }
		
		#endregion

		#region Error Checking

        /// <summary>
        /// Gets the error description for a given error code.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <returns>The error description for the given error code.</returns>
        public static string GetErrorDescription(uint errorCode)
        {
            switch (errorCode)
            {
                case GL_NO_ERROR:
                    return "No Error";
                case GL_INVALID_ENUM:
                    return "A GLenum argument was out of range.";
                case GL_INVALID_VALUE:
                    return "A numeric argument was out of range.";
                case GL_INVALID_OPERATION:
                    return "Invalid operation.";
                case GL_STACK_OVERFLOW:
                    return "Command would cause a stack overflow.";
                case GL_STACK_UNDERFLOW:
                    return "Command would cause a stack underflow.";
                case GL_OUT_OF_MEMORY:
                    return "Not enough memory left to execute command.";
                default:
                    return "Unknown Error";
            }
        }

        /// <summary>
        /// Called before an OpenGL call to enable error checking and ensure the
        /// correct OpenGL context is correct.
        /// </summary>
		private static void PreGLCall()
        {
            //  If we are in debug mode, clear the error flag.
#if DEBUG
            // GetError() should not be called at all inside glBegin-glEnd
            if (insideGLBegin == false)
            {
                GetError();
            }
#endif
        }

        /// <summary>
        /// Called after an OpenGL call to enable error checking.
        /// </summary>
        private static void PostGLCall()
        {
#if DEBUG
            //  We can only perform the following error check if we
            //  are not in a glBegin function.
            if (insideGLBegin == false)
            {
                //	This error check is very useful, as you can break anytime 
                //	an OpenGL error occurs, going through a program with this on
                //	can rid it of bugs. It's VERY slow though, as every call is monitored.
                uint errorCode = GetError();

                //	What error is it?
                if (errorCode != GL_NO_ERROR)
                {
                    //  Get the error message.
                    var errorMessage = GetErrorDescription(errorCode);

                    //  Create a stack trace.
                    var stackTrace = new StackTrace();

                    //  Get the stack frames.
                    var stackFrames = stackTrace.GetFrames();

                    //  Write the error to the trace log.
                    var functionName = (stackFrames != null && stackFrames.Length > 1) ? stackFrames[1].GetMethod().Name : "<Unknown Function>";
                    Trace.WriteLine("OpenGL Error: \"" + errorMessage + "\", when calling function NaGL." + functionName);
                }
            }
#endif
        }

        #endregion

        #region Extensions

        /// <summary>
        /// Determines whether a named extension function is supported.
        /// </summary>
        /// <param name="extensionFunctionName">Name of the extension function.</param>
        /// <returns>
        /// 	<c>true</c> if the extension function is supported; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsExtensionFunctionSupported(string extensionFunctionName)
        {
            //  Try and get the proc address for the function.
            IntPtr procAddress = Platform.GlGetProcAddress(extensionFunctionName);

            //  As long as the pointer is non-zero, we can invoke the extension function.
            return procAddress != IntPtr.Zero;
        }

        /// <summary>
        /// Returns a delegate for an extension function. This delegate  can be called to execute the extension function.
        /// </summary>
        /// <typeparam name="T">The extension delegate type.</typeparam>
        /// <returns>The delegate that points to the extension function.</returns>
        private static T GetDelegateFor<T>() where T : class
        {
            //  Get the type of the extension function.
            Type delegateType = typeof(T);

            //  Get the name of the extension function.
            string name = delegateType.Name;

            // ftlPhysicsGuy - Better way
            Delegate del = null;
            if (extensionFunctions.TryGetValue(name, out del) == false)
            {
                IntPtr proc = IntPtr.Zero;
                if (name.EndsWith("EXT")) {
                    proc = Platform.GlGetProcAddress(name.Substring(0, name.Length - 3));
                }

                if (proc == IntPtr.Zero)
                    proc = Platform.GlGetProcAddress(name);

                if (proc == IntPtr.Zero)
                    throw new Exception("Extension function " + name + " not supported");

                //  Get the delegate for the function pointer.
                del = Marshal.GetDelegateForFunctionPointer(proc, delegateType);

                //  Add to the dictionary.
                extensionFunctions.Add(name, del);
            }

            return del as T;
        }

        /// <summary>
        /// The set of extension functions.
        /// </summary>
        private static Dictionary<string, Delegate> extensionFunctions = new Dictionary<string, Delegate>();

        #region TODO:

        //Methods
        public static void BindTextureUnit(uint textureUnit, Texture texture) => BindTextureUnit(textureUnit, texture.Handle);
        public static void BindTextureUnit(uint textureUnit, uint texture)
        {
            GetDelegateFor<glBindTextureUnit>()(textureUnit, texture);
        }
        
        private delegate void glBindTextureUnit(uint textureUnit, uint texture);

        #endregion

        #region OpenGL 1.2

        //  Methods
        public static void BlendColor(float red, float green, float blue, float alpha)
        {
            GetDelegateFor<glBlendColor>()(red, green, blue, alpha);
        }
        public static void BlendEquation(uint mode)
        {
            GetDelegateFor<glBlendEquation>()(mode);
        }
        public static void DrawRangeElements(uint mode, uint start, uint end, int count, uint type, IntPtr indices)
        {
            GetDelegateFor<glDrawRangeElements>()(mode, start, end, count, type, indices);
        }
        public static void TexImage3D(uint target, int level, int internalformat, int width, int height, int depth, int border, uint format, uint type, IntPtr pixels)
        {
            GetDelegateFor<glTexImage3D>()(target, level, internalformat, width, height, depth, border, format, type, pixels);
        }
        public static void TexSubImage3D(uint target, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, uint format, uint type, IntPtr pixels)
        {
            GetDelegateFor<glTexSubImage3D>()(target, level, xoffset, yoffset, zoffset, width, height, depth, format, type, pixels);
        }
        public static void CopyTexSubImage3D(uint target, int level, int xoffset, int yoffset, int zoffset, int x, int y, int width, int height)
        {
            GetDelegateFor<glCopyTexSubImage3D>()(target, level, xoffset, yoffset, zoffset, x, y, width, height);
        }

        //  Deprecated Methods
        [Obsolete]
        public static void ColorTable(uint target, uint internalformat, int width, uint format, uint type, IntPtr table)
        {
            GetDelegateFor<glColorTable>()(target, internalformat, width, format, type, table);
        }
        [Obsolete]
        public static void ColorTableParameterfv(uint target, uint pname, float[] parameters)
        {
            GetDelegateFor<glColorTableParameterfv>()(target, pname, parameters);
        }
        [Obsolete]
        public static void ColorTableParameteriv(uint target, uint pname, int[] parameters)
        {
            GetDelegateFor<glColorTableParameteriv>()(target, pname, parameters);
        }
        [Obsolete]
        public static void CopyColorTable(uint target, uint internalformat, int x, int y, int width)
        {
            GetDelegateFor<glCopyColorTable>()(target, internalformat, x, y, width);
        }
        [Obsolete]
        public static void GetColorTable(uint target, uint format, uint type, IntPtr table)
        {
            GetDelegateFor<glGetColorTable>()(target, format, type, table);
        }
        [Obsolete]
        public static void GetColorTableParameter(uint target, uint pname, float[] parameters)
        {
            GetDelegateFor<glGetColorTableParameterfv>()(target, pname, parameters);
        }
        [Obsolete]
        public static void GetColorTableParameter(uint target, uint pname, int[] parameters)
        {
            GetDelegateFor<glGetColorTableParameteriv>()(target, pname, parameters);
        }
        [Obsolete]
        public static void ColorSubTable(uint target, int start, int count, uint format, uint type, IntPtr data)
        {
            GetDelegateFor<glColorSubTable>()(target, start, count, format, type, data);
        }
        [Obsolete]
        public static void CopyColorSubTable(uint target, int start, int x, int y, int width)
        {
            GetDelegateFor<glCopyColorSubTable>()(target, start, x, y, width);
        }
        [Obsolete]
        public static void ConvolutionFilter1D(uint target, uint internalformat, int width, uint format, uint type, IntPtr image)
        {
            GetDelegateFor<glConvolutionFilter1D>()(target, internalformat, width, format, type, image);
        }
        [Obsolete]
        public static void ConvolutionFilter2D(uint target, uint internalformat, int width, int height, uint format, uint type, IntPtr image)
        {
            GetDelegateFor<glConvolutionFilter2D>()(target, internalformat, width, height, format, type, image);
        }
        [Obsolete]
        public static void ConvolutionParameter(uint target, uint pname, float parameters)
        {
            GetDelegateFor<glConvolutionParameterf>()(target, pname, parameters);
        }
        [Obsolete]
        public static void ConvolutionParameter(uint target, uint pname, float[] parameters)
        {
            GetDelegateFor<glConvolutionParameterfv>()(target, pname, parameters);
        }
        [Obsolete]
        public static void ConvolutionParameter(uint target, uint pname, int parameters)
        {
            GetDelegateFor<glConvolutionParameteri>()(target, pname, parameters);
        }
        [Obsolete]
        public static void ConvolutionParameter(uint target, uint pname, int[] parameters)
        {
            GetDelegateFor<glConvolutionParameteriv>()(target, pname, parameters);
        }
        [Obsolete]
        public static void CopyConvolutionFilter1D(uint target, uint internalformat, int x, int y, int width)
        {
            GetDelegateFor<glCopyConvolutionFilter1D>()(target, internalformat, x, y, width);
        }
        [Obsolete]
        public static void CopyConvolutionFilter2D(uint target, uint internalformat, int x, int y, int width, int height)
        {
            GetDelegateFor<glCopyConvolutionFilter2D>()(target, internalformat, x, y, width, height);
        }
        [Obsolete]
        public static void GetConvolutionFilter(uint target, uint format, uint type, IntPtr image)
        {
            GetDelegateFor<glGetConvolutionFilter>()(target, format, type, image);
        }
        [Obsolete]
        public static void GetConvolutionParameter(uint target, uint pname, float[] parameters)
        {
            GetDelegateFor<glGetConvolutionParameterfv>()(target, pname, parameters);
        }
        [Obsolete]
        public static void GetConvolutionParameter(uint target, uint pname, int[] parameters)
        {
            GetDelegateFor<glGetConvolutionParameteriv>()(target, pname, parameters);
        }
        [Obsolete]
        public static void GetSeparableFilter(uint target, uint format, uint type, IntPtr row, IntPtr column, IntPtr span)
        {
            GetDelegateFor<glGetSeparableFilter>()(target, format, type, row, column, span);
        }
        [Obsolete]
        public static void SeparableFilter2D(uint target, uint internalformat, int width, int height, uint format, uint type, IntPtr row, IntPtr column)
        {
            GetDelegateFor<glSeparableFilter2D>()(target, internalformat, width, height, format, type, row, column);
        }
        [Obsolete]
        public static void GetHistogram(uint target, bool reset, uint format, uint type, IntPtr values)
        {
            GetDelegateFor<glGetHistogram>()(target, reset, format, type, values);
        }
        [Obsolete]
        public static void GetHistogramParameter(uint target, uint pname, float[] parameters)
        {
            GetDelegateFor<glGetHistogramParameterfv>()(target, pname, parameters);
        }
        [Obsolete]
        public static void GetHistogramParameter(uint target, uint pname, int[] parameters)
        {
            GetDelegateFor<glGetHistogramParameteriv>()(target, pname, parameters);
        }
        [Obsolete]
        public static void GetMinmax(uint target, bool reset, uint format, uint type, IntPtr values)
        {
            GetDelegateFor<glGetMinmax>()(target, reset, format, type, values);
        }
        [Obsolete]
        public static void GetMinmaxParameter(uint target, uint pname, float[] parameters)
        {
            GetDelegateFor<glGetMinmaxParameterfv>()(target, pname, parameters);
        }
        [Obsolete]
        public static void GetMinmaxParameter(uint target, uint pname, int[] parameters)
        {
            GetDelegateFor<glGetMinmaxParameteriv>()(target, pname, parameters);
        }
        [Obsolete]
        public static void Histogram(uint target, int width, uint internalformat, bool sink)
        {
            GetDelegateFor<glHistogram>()(target, width, internalformat, sink);
        }
        [Obsolete]
        public static void Minmax(uint target, uint internalformat, bool sink)
        {
            GetDelegateFor<glMinmax>()(target, internalformat, sink);
        }
        [Obsolete]
        public static void ResetHistogram(uint target)
        {
            GetDelegateFor<glResetHistogram>()(target);
        }
        [Obsolete]
        public static void ResetMinmax(uint target)
        {
            GetDelegateFor<glResetMinmax>()(target);
        }

        //  Delegates
        private delegate void glBlendColor(float red, float green, float blue, float alpha);
        private delegate void glBlendEquation(uint mode);
        private delegate void glDrawRangeElements(uint mode, uint start, uint end, int count, uint type, IntPtr indices);
        private delegate void glTexImage3D(uint target, int level, int internalformat, int width, int height, int depth, int border, uint format, uint type, IntPtr pixels);
        private delegate void glTexSubImage3D(uint target, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, uint format, uint type, IntPtr pixels);
        private delegate void glCopyTexSubImage3D(uint target, int level, int xoffset, int yoffset, int zoffset, int x, int y, int width, int height);
        private delegate void glColorTable(uint target, uint internalformat, int width, uint format, uint type, IntPtr table);
        private delegate void glColorTableParameterfv(uint target, uint pname, float[] parameters);
        private delegate void glColorTableParameteriv(uint target, uint pname, int[] parameters);
        private delegate void glCopyColorTable(uint target, uint internalformat, int x, int y, int width);
        private delegate void glGetColorTable(uint target, uint format, uint type, IntPtr table);
        private delegate void glGetColorTableParameterfv(uint target, uint pname, float[] parameters);
        private delegate void glGetColorTableParameteriv(uint target, uint pname, int[] parameters);
        private delegate void glColorSubTable(uint target, int start, int count, uint format, uint type, IntPtr data);
        private delegate void glCopyColorSubTable(uint target, int start, int x, int y, int width);
        private delegate void glConvolutionFilter1D(uint target, uint internalformat, int width, uint format, uint type, IntPtr image);
        private delegate void glConvolutionFilter2D(uint target, uint internalformat, int width, int height, uint format, uint type, IntPtr image);
        private delegate void glConvolutionParameterf(uint target, uint pname, float parameters);
        private delegate void glConvolutionParameterfv(uint target, uint pname, float[] parameters);
        private delegate void glConvolutionParameteri(uint target, uint pname, int parameters);
        private delegate void glConvolutionParameteriv(uint target, uint pname, int[] parameters);
        private delegate void glCopyConvolutionFilter1D(uint target, uint internalformat, int x, int y, int width);
        private delegate void glCopyConvolutionFilter2D(uint target, uint internalformat, int x, int y, int width, int height);
        private delegate void glGetConvolutionFilter(uint target, uint format, uint type, IntPtr image);
        private delegate void glGetConvolutionParameterfv(uint target, uint pname, float[] parameters);
        private delegate void glGetConvolutionParameteriv(uint target, uint pname, int[] parameters);
        private delegate void glGetSeparableFilter(uint target, uint format, uint type, IntPtr row, IntPtr column, IntPtr span);
        private delegate void glSeparableFilter2D(uint target, uint internalformat, int width, int height, uint format, uint type, IntPtr row, IntPtr column);
        private delegate void glGetHistogram(uint target, bool reset, uint format, uint type, IntPtr values);
        private delegate void glGetHistogramParameterfv(uint target, uint pname, float[] parameters);
        private delegate void glGetHistogramParameteriv(uint target, uint pname, int[] parameters);
        private delegate void glGetMinmax(uint target, bool reset, uint format, uint type, IntPtr values);
        private delegate void glGetMinmaxParameterfv(uint target, uint pname, float[] parameters);
        private delegate void glGetMinmaxParameteriv(uint target, uint pname, int[] parameters);
        private delegate void glHistogram(uint target, int width, uint internalformat, bool sink);
        private delegate void glMinmax(uint target, uint internalformat, bool sink);
        private delegate void glResetHistogram(uint target);
        private delegate void glResetMinmax(uint target);

        //  Constants
        public const uint GL_UNSIGNED_BYTE_3_3_2 = 0x8032;
        public const uint GL_UNSIGNED_SHORT_4_4_4_4 = 0x8033;
        public const uint GL_UNSIGNED_SHORT_5_5_5_1 = 0x8034;
        public const uint GL_UNSIGNED_INT_8_8_8_8 = 0x8035;
        public const uint GL_UNSIGNED_INT_10_10_10_2 = 0x8036;
        public const uint GL_TEXTURE_BINDING_3D = 0x806A;
        public const uint GL_PACK_SKIP_IMAGES = 0x806B;
        public const uint GL_PACK_IMAGE_HEIGHT = 0x806C;
        public const uint GL_UNPACK_SKIP_IMAGES = 0x806D;
        public const uint GL_UNPACK_IMAGE_HEIGHT = 0x806E;
        public const uint GL_TEXTURE_3D = 0x806F;
        public const uint GL_PROXY_TEXTURE_3D = 0x8070;
        public const uint GL_TEXTURE_DEPTH = 0x8071;
        public const uint GL_TEXTURE_WRAP_R = 0x8072;
        public const uint GL_MAX_3D_TEXTURE_SIZE = 0x8073;
        public const uint GL_UNSIGNED_BYTE_2_3_3_REV = 0x8362;
        public const uint GL_UNSIGNED_SHORT_5_6_5 = 0x8363;
        public const uint GL_UNSIGNED_SHORT_5_6_5_REV = 0x8364;
        public const uint GL_UNSIGNED_SHORT_4_4_4_4_REV = 0x8365;
        public const uint GL_UNSIGNED_SHORT_1_5_5_5_REV = 0x8366;
        public const uint GL_UNSIGNED_INT_8_8_8_8_REV = 0x8367;
        public const uint GL_UNSIGNED_INT_2_10_10_10_REV = 0x8368;
        public const uint GL_BGR = 0x80E0;
        public const uint GL_BGRA = 0x80E1;
        public const uint GL_MAX_ELEMENTS_VERTICES = 0x80E8;
        public const uint GL_MAX_ELEMENTS_INDICES = 0x80E9;
        public const uint GL_CLAMP_TO_EDGE = 0x812F;
        public const uint GL_TEXTURE_MIN_LOD = 0x813A;
        public const uint GL_TEXTURE_MAX_LOD = 0x813B;
        public const uint GL_TEXTURE_BASE_LEVEL = 0x813C;
        public const uint GL_TEXTURE_MAX_LEVEL = 0x813D;
        public const uint GL_SMOOTH_POINT_SIZE_RANGE = 0x0B12;
        public const uint GL_SMOOTH_POINT_SIZE_GRANULARITY = 0x0B13;
        public const uint GL_SMOOTH_LINE_WIDTH_RANGE = 0x0B22;
        public const uint GL_SMOOTH_LINE_WIDTH_GRANULARITY = 0x0B23;
        public const uint GL_ALIASED_LINE_WIDTH_RANGE = 0x846E;

        #endregion

        #region OpenGL 1.3

        //  Methods


        public static void ActiveTexture(uint texture)
        {
            GetDelegateFor<glActiveTexture>()(texture);
        }
        public static void SampleCoverage(float value, bool invert)
        {
            GetDelegateFor<glSampleCoverage>()(value, invert);
        }
        public static void CompressedTexImage3D(uint target, int level, uint internalformat, int width, int height, int depth, int border, int imageSize, IntPtr data)
        {
            GetDelegateFor<glCompressedTexImage3D>()(target, level, internalformat, width, height, depth, border, imageSize, data);
        }
        public static void CompressedTexImage2D(uint target, int level, uint internalformat, int width, int height, int border, int imageSize, IntPtr data)
        {
            GetDelegateFor<glCompressedTexImage2D>()(target, level, internalformat, width, height, border, imageSize, data);
        }
        public static void CompressedTexImage1D(uint target, int level, uint internalformat, int width, int border, int imageSize, IntPtr data)
        {
            GetDelegateFor<glCompressedTexImage1D>()(target, level, internalformat, width, border, imageSize, data);
        }
        public static void CompressedTexSubImage3D(uint target, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, uint format, int imageSize, IntPtr data)
        {
            GetDelegateFor<glCompressedTexSubImage3D>()(target, level, xoffset, yoffset, zoffset, width, height, depth, format, imageSize, data);
        }
        public static void CompressedTexSubImage2D(uint target, int level, int xoffset, int yoffset, int width, int height, uint format, int imageSize, IntPtr data)
        {
            GetDelegateFor<glCompressedTexSubImage2D>()(target, level, xoffset, yoffset, width, height, format, imageSize, data);
        }
        public static void CompressedTexSubImage1D(uint target, int level, int xoffset, int width, uint format, int imageSize, IntPtr data)
        {
            GetDelegateFor<glCompressedTexSubImage1D>()(target, level, xoffset, width, format, imageSize, data);
        }
        public static void GetCompressedTexImage(uint target, int level, IntPtr img)
        {
            GetDelegateFor<glGetCompressedTexImage>()(target, level, img);
        }

        //  Deprecated Methods
        [Obsolete]
        public static void ClientActiveTexture(uint texture)
        {
            GetDelegateFor<glClientActiveTexture>()(texture);
        }
        [Obsolete]
        public static void MultiTexCoord1(uint target, double s)
        {
            GetDelegateFor<glMultiTexCoord1d>()(target, s);
        }
        [Obsolete]
        public static void MultiTexCoord1(uint target, double[] v)
        {
            GetDelegateFor<glMultiTexCoord1dv>()(target, v);
        }
        [Obsolete]
        public static void MultiTexCoord1(uint target, float s)
        {
            GetDelegateFor<glMultiTexCoord1f>()(target, s);
        }
        [Obsolete]
        public static void MultiTexCoord1(uint target, float[] v)
        {
            GetDelegateFor<glMultiTexCoord1fv>()(target, v);
        }
        [Obsolete]
        public static void MultiTexCoord1(uint target, int s)
        {
            GetDelegateFor<glMultiTexCoord1i>()(target, s);
        }
        [Obsolete]
        public static void MultiTexCoord1(uint target, int[] v)
        {
            GetDelegateFor<glMultiTexCoord1iv>()(target, v);
        }
        [Obsolete]
        public static void MultiTexCoord1(uint target, short s)
        {
            GetDelegateFor<glMultiTexCoord1s>()(target, s);
        }
        [Obsolete]
        public static void MultiTexCoord1(uint target, short[] v)
        {
            GetDelegateFor<glMultiTexCoord1sv>()(target, v);
        }
        [Obsolete]
        public static void MultiTexCoord2(uint target, double s, double t)
        {
            GetDelegateFor<glMultiTexCoord2d>()(target, s, t);
        }
        [Obsolete]
        public static void MultiTexCoord2(uint target, double[] v)
        {
            GetDelegateFor<glMultiTexCoord2dv>()(target, v);
        }
        [Obsolete]
        public static void MultiTexCoord2(uint target, float s, float t)
        {
            GetDelegateFor<glMultiTexCoord2f>()(target, s, t);
        }
        [Obsolete]
        public static void MultiTexCoord2(uint target, float[] v)
        {
            GetDelegateFor<glMultiTexCoord2fv>()(target, v);
        }
        [Obsolete]
        public static void MultiTexCoord2(uint target, int s, int t)
        {
            GetDelegateFor<glMultiTexCoord2i>()(target, s, t);
        }
        [Obsolete]
        public static void MultiTexCoord2(uint target, int[] v)
        {
            GetDelegateFor<glMultiTexCoord2iv>()(target, v);
        }
        [Obsolete]
        public static void MultiTexCoord2(uint target, short s, short t)
        {
            GetDelegateFor<glMultiTexCoord2s>()(target, s, t);
        }
        [Obsolete]
        public static void MultiTexCoord2(uint target, short[] v)
        {
            GetDelegateFor<glMultiTexCoord2sv>()(target, v);
        }
        [Obsolete]
        public static void MultiTexCoord3(uint target, double s, double t, double r)
        {
            GetDelegateFor<glMultiTexCoord3d>()(target, s, t, r);
        }
        [Obsolete]
        public static void MultiTexCoord3(uint target, double[] v)
        {
            GetDelegateFor<glMultiTexCoord3dv>()(target, v);
        }
        [Obsolete]
        public static void MultiTexCoord3(uint target, float s, float t, float r)
        {
            GetDelegateFor<glMultiTexCoord3f>()(target, s, t, r);
        }
        [Obsolete]
        public static void MultiTexCoord3(uint target, float[] v)
        {
            GetDelegateFor<glMultiTexCoord3fv>()(target, v);
        }
        [Obsolete]
        public static void MultiTexCoord3(uint target, int s, int t, int r)
        {
            GetDelegateFor<glMultiTexCoord3i>()(target, s, t, r);
        }
        [Obsolete]
        public static void MultiTexCoord3(uint target, int[] v)
        {
            GetDelegateFor<glMultiTexCoord3iv>()(target, v);
        }
        [Obsolete]
        public static void MultiTexCoord3(uint target, short s, short t, short r)
        {
            GetDelegateFor<glMultiTexCoord3s>()(target, s, t, r);
        }
        [Obsolete]
        public static void MultiTexCoord3(uint target, short[] v)
        {
            GetDelegateFor<glMultiTexCoord3sv>()(target, v);
        }
        [Obsolete]
        public static void MultiTexCoord4(uint target, double s, double t, double r, double q)
        {
            GetDelegateFor<glMultiTexCoord4d>()(target, s, t, r, q);
        }
        [Obsolete]
        public static void MultiTexCoord4(uint target, double[] v)
        {
            GetDelegateFor<glMultiTexCoord4dv>()(target, v);
        }
        [Obsolete]
        public static void MultiTexCoord4(uint target, float s, float t, float r, float q)
        {
            GetDelegateFor<glMultiTexCoord4f>()(target, s, t, r, q);
        }
        [Obsolete]
        public static void MultiTexCoord4(uint target, float[] v)
        {
            GetDelegateFor<glMultiTexCoord4fv>()(target, v);
        }
        [Obsolete]
        public static void MultiTexCoord4(uint target, int s, int t, int r, int q)
        {
            GetDelegateFor<glMultiTexCoord4i>()(target, s, t, r, q);
        }
        [Obsolete]
        public static void MultiTexCoord4(uint target, int[] v)
        {
            GetDelegateFor<glMultiTexCoord4iv>()(target, v);
        }
        [Obsolete]
        public static void MultiTexCoord4(uint target, short s, short t, short r, short q)
        {
            GetDelegateFor<glMultiTexCoord4s>()(target, s, t, r, q);
        }
        [Obsolete]
        public static void MultiTexCoord4(uint target, short[] v)
        {
            GetDelegateFor<glMultiTexCoord4sv>()(target, v);
        }
        [Obsolete]
        public static void LoadTransposeMatrix(float[] m)
        {
            GetDelegateFor<glLoadTransposeMatrixf>()(m);
        }
        [Obsolete]
        public static void LoadTransposeMatrix(double[] m)
        {
            GetDelegateFor<glLoadTransposeMatrixd>()(m);
        }
        [Obsolete]
        public static void MultTransposeMatrix(float[] m)
        {
            GetDelegateFor<glMultTransposeMatrixf>()(m);
        }
        [Obsolete]
        public static void MultTransposeMatrix(double[] m)
        {
            GetDelegateFor<glMultTransposeMatrixd>()(m);
        }

        //  Delegates
        private delegate void glActiveTexture(uint texture);
        private delegate void glSampleCoverage(float value, bool invert);
        private delegate void glCompressedTexImage3D(uint target, int level, uint internalformat, int width, int height, int depth, int border, int imageSize, IntPtr data);
        private delegate void glCompressedTexImage2D(uint target, int level, uint internalformat, int width, int height, int border, int imageSize, IntPtr data);
        private delegate void glCompressedTexImage1D(uint target, int level, uint internalformat, int width, int border, int imageSize, IntPtr data);
        private delegate void glCompressedTexSubImage3D(uint target, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, uint format, int imageSize, IntPtr data);
        private delegate void glCompressedTexSubImage2D(uint target, int level, int xoffset, int yoffset, int width, int height, uint format, int imageSize, IntPtr data);
        private delegate void glCompressedTexSubImage1D(uint target, int level, int xoffset, int width, uint format, int imageSize, IntPtr data);
        private delegate void glGetCompressedTexImage(uint target, int level, IntPtr img);

        private delegate void glClientActiveTexture(uint texture);
        private delegate void glMultiTexCoord1d(uint target, double s);
        private delegate void glMultiTexCoord1dv(uint target, double[] v);
        private delegate void glMultiTexCoord1f(uint target, float s);
        private delegate void glMultiTexCoord1fv(uint target, float[] v);
        private delegate void glMultiTexCoord1i(uint target, int s);
        private delegate void glMultiTexCoord1iv(uint target, int[] v);
        private delegate void glMultiTexCoord1s(uint target, short s);
        private delegate void glMultiTexCoord1sv(uint target, short[] v);
        private delegate void glMultiTexCoord2d(uint target, double s, double t);
        private delegate void glMultiTexCoord2dv(uint target, double[] v);
        private delegate void glMultiTexCoord2f(uint target, float s, float t);
        private delegate void glMultiTexCoord2fv(uint target, float[] v);
        private delegate void glMultiTexCoord2i(uint target, int s, int t);
        private delegate void glMultiTexCoord2iv(uint target, int[] v);
        private delegate void glMultiTexCoord2s(uint target, short s, short t);
        private delegate void glMultiTexCoord2sv(uint target, short[] v);
        private delegate void glMultiTexCoord3d(uint target, double s, double t, double r);
        private delegate void glMultiTexCoord3dv(uint target, double[] v);
        private delegate void glMultiTexCoord3f(uint target, float s, float t, float r);
        private delegate void glMultiTexCoord3fv(uint target, float[] v);
        private delegate void glMultiTexCoord3i(uint target, int s, int t, int r);
        private delegate void glMultiTexCoord3iv(uint target, int[] v);
        private delegate void glMultiTexCoord3s(uint target, short s, short t, short r);
        private delegate void glMultiTexCoord3sv(uint target, short[] v);
        private delegate void glMultiTexCoord4d(uint target, double s, double t, double r, double q);
        private delegate void glMultiTexCoord4dv(uint target, double[] v);
        private delegate void glMultiTexCoord4f(uint target, float s, float t, float r, float q);
        private delegate void glMultiTexCoord4fv(uint target, float[] v);
        private delegate void glMultiTexCoord4i(uint target, int s, int t, int r, int q);
        private delegate void glMultiTexCoord4iv(uint target, int[] v);
        private delegate void glMultiTexCoord4s(uint target, short s, short t, short r, short q);
        private delegate void glMultiTexCoord4sv(uint target, short[] v);
        private delegate void glLoadTransposeMatrixf(float[] m);
        private delegate void glLoadTransposeMatrixd(double[] m);
        private delegate void glMultTransposeMatrixf(float[] m);
        private delegate void glMultTransposeMatrixd(double[] m);

        //  Constants
        public const uint GL_TEXTURE0 = 0x84C0;
        public const uint GL_TEXTURE1 = 0x84C1;
        public const uint GL_TEXTURE2 = 0x84C2;
        public const uint GL_TEXTURE3 = 0x84C3;
        public const uint GL_TEXTURE4 = 0x84C4;
        public const uint GL_TEXTURE5 = 0x84C5;
        public const uint GL_TEXTURE6 = 0x84C6;
        public const uint GL_TEXTURE7 = 0x84C7;
        public const uint GL_TEXTURE8 = 0x84C8;
        public const uint GL_TEXTURE9 = 0x84C9;
        public const uint GL_TEXTURE10 = 0x84CA;
        public const uint GL_TEXTURE11 = 0x84CB;
        public const uint GL_TEXTURE12 = 0x84CC;
        public const uint GL_TEXTURE13 = 0x84CD;
        public const uint GL_TEXTURE14 = 0x84CE;
        public const uint GL_TEXTURE15 = 0x84CF;
        public const uint GL_TEXTURE16 = 0x84D0;
        public const uint GL_TEXTURE17 = 0x84D1;
        public const uint GL_TEXTURE18 = 0x84D2;
        public const uint GL_TEXTURE19 = 0x84D3;
        public const uint GL_TEXTURE20 = 0x84D4;
        public const uint GL_TEXTURE21 = 0x84D5;
        public const uint GL_TEXTURE22 = 0x84D6;
        public const uint GL_TEXTURE23 = 0x84D7;
        public const uint GL_TEXTURE24 = 0x84D8;
        public const uint GL_TEXTURE25 = 0x84D9;
        public const uint GL_TEXTURE26 = 0x84DA;
        public const uint GL_TEXTURE27 = 0x84DB;
        public const uint GL_TEXTURE28 = 0x84DC;
        public const uint GL_TEXTURE29 = 0x84DD;
        public const uint GL_TEXTURE30 = 0x84DE;
        public const uint GL_TEXTURE31 = 0x84DF;
        public const uint GL_ACTIVE_TEXTURE = 0x84E0;
        public const uint GL_MULTISAMPLE = 0x809D;
        public const uint GL_SAMPLE_ALPHA_TO_COVERAGE = 0x809E;
        public const uint GL_SAMPLE_ALPHA_TO_ONE = 0x809F;
        public const uint GL_SAMPLE_COVERAGE = 0x80A0;
        public const uint GL_SAMPLE_BUFFERS = 0x80A8;
        public const uint GL_SAMPLES = 0x80A9;
        public const uint GL_SAMPLE_COVERAGE_VALUE = 0x80AA;
        public const uint GL_SAMPLE_COVERAGE_INVERT = 0x80AB;
        public const uint GL_TEXTURE_CUBE_MAP = 0x8513;
        public const uint GL_TEXTURE_BINDING_CUBE_MAP = 0x8514;
        public const uint GL_TEXTURE_CUBE_MAP_POSITIVE_X = 0x8515;
        public const uint GL_TEXTURE_CUBE_MAP_NEGATIVE_X = 0x8516;
        public const uint GL_TEXTURE_CUBE_MAP_POSITIVE_Y = 0x8517;
        public const uint GL_TEXTURE_CUBE_MAP_NEGATIVE_Y = 0x8518;
        public const uint GL_TEXTURE_CUBE_MAP_POSITIVE_Z = 0x8519;
        public const uint GL_TEXTURE_CUBE_MAP_NEGATIVE_Z = 0x851A;
        public const uint GL_PROXY_TEXTURE_CUBE_MAP = 0x851B;
        public const uint GL_MAX_CUBE_MAP_TEXTURE_SIZE = 0x851C;
        public const uint GL_COMPRESSED_RGB = 0x84ED;
        public const uint GL_COMPRESSED_RGBA = 0x84EE;
        public const uint GL_TEXTURE_COMPRESSION_HINT = 0x84EF;
        public const uint GL_TEXTURE_COMPRESSED_IMAGE_SIZE = 0x86A0;
        public const uint GL_TEXTURE_COMPRESSED = 0x86A1;
        public const uint GL_NUM_COMPRESSED_TEXTURE_FORMATS = 0x86A2;
        public const uint GL_COMPRESSED_TEXTURE_FORMATS = 0x86A3;
        public const uint GL_CLAMP_TO_BORDER = 0x812D;

        #endregion

        #region OpenGL 1.4

        //  Methods
        public static void BlendFuncSeparate(uint sfactorRGB, uint dfactorRGB, uint sfactorAlpha, uint dfactorAlpha)
        {
            GetDelegateFor<glBlendFuncSeparate>()(sfactorRGB, dfactorRGB, sfactorAlpha, dfactorAlpha);
        }
        public static void MultiDrawArrays(uint mode, int[] first, int[] count, int primcount)
        {
            GetDelegateFor<glMultiDrawArrays>()(mode, first, count, primcount);
        }
        public static void MultiDrawElements(uint mode, int[] count, uint type, IntPtr indices, int primcount)
        {
            GetDelegateFor<glMultiDrawElements>()(mode, count, type, indices, primcount);
        }
        public static void PointParameter(uint pname, float parameter)
        {
            GetDelegateFor<glPointParameterf>()(pname, parameter);
        }
        public static void PointParameter(uint pname, float[] parameters)
        {
            GetDelegateFor<glPointParameterfv>()(pname, parameters);
        }
        public static void PointParameter(uint pname, int parameter)
        {
            GetDelegateFor<glPointParameteri>()(pname, parameter);
        }
        public static void PointParameter(uint pname, int[] parameters)
        {
            GetDelegateFor<glPointParameteriv>()(pname, parameters);
        }

        //  Deprecated Methods
        [Obsolete]
        public static void FogCoord(float coord)
        {
            GetDelegateFor<glFogCoordf>()(coord);
        }
        [Obsolete]
        public static void FogCoord(float[] coord)
        {
            GetDelegateFor<glFogCoordfv>()(coord);
        }
        [Obsolete]
        public static void FogCoord(double coord)
        {
            GetDelegateFor<glFogCoordd>()(coord);
        }
        [Obsolete]
        public static void FogCoord(double[] coord)
        {
            GetDelegateFor<glFogCoorddv>()(coord);
        }
        [Obsolete]
        public static void FogCoordPointer(uint type, int stride, IntPtr pointer)
        {
            GetDelegateFor<glFogCoordPointer>()(type, stride, pointer);
        }
        [Obsolete]
        public static void SecondaryColor3(sbyte red, sbyte green, sbyte blue)
        {
            GetDelegateFor<glSecondaryColor3b>()(red, green, blue);
        }
        [Obsolete]
        public static void SecondaryColor3(sbyte[] v)
        {
            GetDelegateFor<glSecondaryColor3bv>()(v);
        }
        [Obsolete]
        public static void SecondaryColor3(double red, double green, double blue)
        {
            GetDelegateFor<glSecondaryColor3d>()(red, green, blue);
        }
        [Obsolete]
        public static void SecondaryColor3(double[] v)
        {
            GetDelegateFor<glSecondaryColor3dv>()(v);
        }
        [Obsolete]
        public static void SecondaryColor3(float red, float green, float blue)
        {
            GetDelegateFor<glSecondaryColor3f>()(red, green, blue);
        }
        [Obsolete]
        public static void SecondaryColor3(float[] v)
        {
            GetDelegateFor<glSecondaryColor3fv>()(v);
        }
        [Obsolete]
        public static void SecondaryColor3(int red, int green, int blue)
        {
            GetDelegateFor<glSecondaryColor3i>()(red, green, blue);
        }
        [Obsolete]
        public static void SecondaryColor3(int[] v)
        {
            GetDelegateFor<glSecondaryColor3iv>()(v);
        }
        [Obsolete]
        public static void SecondaryColor3(short red, short green, short blue)
        {
            GetDelegateFor<glSecondaryColor3s>()(red, green, blue);
        }
        [Obsolete]
        public static void SecondaryColor3(short[] v)
        {
            GetDelegateFor<glSecondaryColor3sv>()(v);
        }
        [Obsolete]
        public static void SecondaryColor3(byte red, byte green, byte blue)
        {
            GetDelegateFor<glSecondaryColor3ub>()(red, green, blue);
        }
        [Obsolete]
        public static void SecondaryColor3(byte[] v)
        {
            GetDelegateFor<glSecondaryColor3ubv>()(v);
        }
        [Obsolete]
        public static void SecondaryColor3(uint red, uint green, uint blue)
        {
            GetDelegateFor<glSecondaryColor3ui>()(red, green, blue);
        }
        [Obsolete]
        public static void SecondaryColor3(uint[] v)
        {
            GetDelegateFor<glSecondaryColor3uiv>()(v);
        }
        [Obsolete]
        public static void SecondaryColor3(ushort red, ushort green, ushort blue)
        {
            GetDelegateFor<glSecondaryColor3us>()(red, green, blue);
        }
        [Obsolete]
        public static void SecondaryColor3(ushort[] v)
        {
            GetDelegateFor<glSecondaryColor3usv>()(v);
        }
        [Obsolete]
        public static void SecondaryColorPointer(int size, uint type, int stride, IntPtr pointer)
        {
            GetDelegateFor<glSecondaryColorPointer>()(size, type, stride, pointer);
        }
        [Obsolete]
        public static void WindowPos2(double x, double y)
        {
            GetDelegateFor<glWindowPos2d>()(x, y);
        }
        [Obsolete]
        public static void WindowPos2(double[] v)
        {
            GetDelegateFor<glWindowPos2dv>()(v);
        }
        [Obsolete]
        public static void WindowPos2(float x, float y)
        {
            GetDelegateFor<glWindowPos2f>()(x, y);
        }
        [Obsolete]
        public static void WindowPos2(float[] v)
        {
            GetDelegateFor<glWindowPos2fv>()(v);
        }
        [Obsolete]
        public static void WindowPos2(int x, int y)
        {
            GetDelegateFor<glWindowPos2i>()(x, y);
        }
        [Obsolete]
        public static void WindowPos2(int[] v)
        {
            GetDelegateFor<glWindowPos2iv>()(v);
        }
        [Obsolete]
        public static void WindowPos2(short x, short y)
        {
            GetDelegateFor<glWindowPos2s>()(x, y);
        }
        [Obsolete]
        public static void WindowPos2(short[] v)
        {
            GetDelegateFor<glWindowPos2sv>()(v);
        }
        [Obsolete]
        public static void WindowPos3(double x, double y, double z)
        {
            GetDelegateFor<glWindowPos3d>()(x, y, z);
        }
        [Obsolete]
        public static void WindowPos3(double[] v)
        {
            GetDelegateFor<glWindowPos3dv>()(v);
        }
        [Obsolete]
        public static void WindowPos3(float x, float y, float z)
        {
            GetDelegateFor<glWindowPos3f>()(x, y, z);
        }
        [Obsolete]
        public static void WindowPos3(float[] v)
        {
            GetDelegateFor<glWindowPos3fv>()(v);
        }
        [Obsolete]
        public static void WindowPos3(int x, int y, int z)
        {
            GetDelegateFor<glWindowPos3i>()(x, y, z);
        }
        [Obsolete]
        public static void WindowPos3(int[] v)
        {
            GetDelegateFor<glWindowPos3iv>()(v);
        }
        [Obsolete]
        public static void WindowPos3(short x, short y, short z)
        {
            GetDelegateFor<glWindowPos3s>()(x, y, z);
        }
        [Obsolete]
        public static void WindowPos3(short[] v)
        {
            GetDelegateFor<glWindowPos3sv>()(v);
        }

        //  Delegates
        private delegate void glBlendFuncSeparate(uint sfactorRGB, uint dfactorRGB, uint sfactorAlpha, uint dfactorAlpha);
        private delegate void glMultiDrawArrays(uint mode, int[] first, int[] count, int primcount);
        private delegate void glMultiDrawElements(uint mode, int[] count, uint type, IntPtr indices, int primcount);
        private delegate void glPointParameterf(uint pname, float parameter);
        private delegate void glPointParameterfv(uint pname, float[] parameters);
        private delegate void glPointParameteri(uint pname, int parameter);
        private delegate void glPointParameteriv(uint pname, int[] parameters);
        private delegate void glFogCoordf(float coord);
        private delegate void glFogCoordfv(float[] coord);
        private delegate void glFogCoordd(double coord);
        private delegate void glFogCoorddv(double[] coord);
        private delegate void glFogCoordPointer(uint type, int stride, IntPtr pointer);
        private delegate void glSecondaryColor3b(sbyte red, sbyte green, sbyte blue);
        private delegate void glSecondaryColor3bv(sbyte[] v);
        private delegate void glSecondaryColor3d(double red, double green, double blue);
        private delegate void glSecondaryColor3dv(double[] v);
        private delegate void glSecondaryColor3f(float red, float green, float blue);
        private delegate void glSecondaryColor3fv(float[] v);
        private delegate void glSecondaryColor3i(int red, int green, int blue);
        private delegate void glSecondaryColor3iv(int[] v);
        private delegate void glSecondaryColor3s(short red, short green, short blue);
        private delegate void glSecondaryColor3sv(short[] v);
        private delegate void glSecondaryColor3ub(byte red, byte green, byte blue);
        private delegate void glSecondaryColor3ubv(byte[] v);
        private delegate void glSecondaryColor3ui(uint red, uint green, uint blue);
        private delegate void glSecondaryColor3uiv(uint[] v);
        private delegate void glSecondaryColor3us(ushort red, ushort green, ushort blue);
        private delegate void glSecondaryColor3usv(ushort[] v);
        private delegate void glSecondaryColorPointer(int size, uint type, int stride, IntPtr pointer);
        private delegate void glWindowPos2d(double x, double y);
        private delegate void glWindowPos2dv(double[] v);
        private delegate void glWindowPos2f(float x, float y);
        private delegate void glWindowPos2fv(float[] v);
        private delegate void glWindowPos2i(int x, int y);
        private delegate void glWindowPos2iv(int[] v);
        private delegate void glWindowPos2s(short x, short y);
        private delegate void glWindowPos2sv(short[] v);
        private delegate void glWindowPos3d(double x, double y, double z);
        private delegate void glWindowPos3dv(double[] v);
        private delegate void glWindowPos3f(float x, float y, float z);
        private delegate void glWindowPos3fv(float[] v);
        private delegate void glWindowPos3i(int x, int y, int z);
        private delegate void glWindowPos3iv(int[] v);
        private delegate void glWindowPos3s(short x, short y, short z);
        private delegate void glWindowPos3sv(short[] v);

        //  Constants
        public const uint GL_BLEND_DST_RGB = 0x80C8;
        public const uint GL_BLEND_SRC_RGB = 0x80C9;
        public const uint GL_BLEND_DST_ALPHA = 0x80CA;
        public const uint GL_BLEND_SRC_ALPHA = 0x80CB;
        public const uint GL_POINT_FADE_THRESHOLD_SIZE = 0x8128;
        public const uint GL_DEPTH_COMPONENT16 = 0x81A5;
        public const uint GL_DEPTH_COMPONENT24 = 0x81A6;
        public const uint GL_DEPTH_COMPONENT32 = 0x81A7;
        public const uint GL_MIRRORED_REPEAT = 0x8370;
        public const uint GL_MAX_TEXTURE_LOD_BIAS = 0x84FD;
        public const uint GL_TEXTURE_LOD_BIAS = 0x8501;
        public const uint GL_INCR_WRAP = 0x8507;
        public const uint GL_DECR_WRAP = 0x8508;
        public const uint GL_TEXTURE_DEPTH_SIZE = 0x884A;
        public const uint GL_TEXTURE_COMPARE_MODE = 0x884C;
        public const uint GL_TEXTURE_COMPARE_FUNC = 0x884D;

        #endregion

        #region OpenGL 1.5

        //  Methods
        public static void GenQueries(int n, uint[] ids)
        {
            GetDelegateFor<glGenQueries>()(n, ids);
        }
        public static void DeleteQueries(int n, uint[] ids)
        {
            GetDelegateFor<glDeleteQueries>()(n, ids);
        }
        public static bool IsQuery(uint id)
        {
            return (bool)GetDelegateFor<glIsQuery>()(id);
        }
        public static void BeginQuery(uint target, uint id)
        {
            GetDelegateFor<glBeginQuery>()(target, id);
        }
        public static void EndQuery(uint target)
        {
            GetDelegateFor<glEndQuery>()(target);
        }
        public static void GetQuery(uint target, uint pname, int[] parameters)
        {
            GetDelegateFor<glGetQueryiv>()(target, pname, parameters);
        }
        public static void GetQueryObject(uint id, uint pname, int[] parameters)
        {
            GetDelegateFor<glGetQueryObjectiv>()(id, pname, parameters);
        }
        public static void GetQueryObject(uint id, uint pname, uint[] parameters)
        {
            GetDelegateFor<glGetQueryObjectuiv>()(id, pname, parameters);
        }
        private static readonly Dictionary<uint, Stack<uint>> bufferHandles = new Dictionary<uint, Stack<uint>>()
        {
            [GL_ARRAY_BUFFER] = new Stack<uint>(),
            [GL_ARRAY_BUFFER_ARB] = new Stack<uint>(),
            [GL_ELEMENT_ARRAY_BUFFER] = new Stack<uint>(),
            [GL_ELEMENT_ARRAY_BUFFER_ARB] = new Stack<uint>(),
        };
        public static void PushArrayBuffer(uint buffer) => PushBuffer(GL_ARRAY_BUFFER, buffer);
        public static void PushElementArrayBuffer(uint buffer) => PushBuffer(GL_ELEMENT_ARRAY_BUFFER, buffer);
        public static void PushBuffer(uint target, uint buffer)
        {
            bufferHandles[target].Push(buffer);
            BindBuffer(target, buffer);
        }
        public static void PopArrayBuffer() => PopBuffer(GL_ARRAY_BUFFER);
        public static void PopElementArrayBuffer() => PopBuffer(GL_ELEMENT_ARRAY_BUFFER);
        public static void PopBuffer(uint target)
        {
            var handles = bufferHandles[target];
            handles.Pop();
            if (handles.Count == 0)
                BindBuffer(target, 0);
            else BindBuffer(target, handles.Peek());
        }
        public static void BindBuffer(uint target, uint buffer)
        {
            GetDelegateFor<glBindBuffer>()(target, buffer);
        }
        public static void DeleteBuffer(uint buffer)
        {
            singleHandle[0] = buffer;
            DeleteBuffers(1, singleHandle);
        }
        public static void DeleteBuffers(int n, uint[] buffers)
        {
            GetDelegateFor<glDeleteBuffers>()(n, buffers);
        }
        public static uint GenBuffer()
        {
            GenBuffers(1, singleHandle);
            return singleHandle[0];
        }
        public static uint[] GenBuffers(int n)
        {
            uint[] buffers = new uint[n];
            GenBuffers(n, buffers);
            return buffers;
        }
        public static void GenBuffers(int n, uint[] buffers)
        {
            GetDelegateFor<glGenBuffers>()(n, buffers);
        }
        public static bool IsBuffer(uint buffer)
        {
            return (bool)GetDelegateFor<glIsBuffer>()(buffer);
        }
        public static void BufferData(uint target, int size, IntPtr data, uint usage)
        {
            GetDelegateFor<glBufferData>()(target, size, data, usage);
        }
        public static void BufferData(uint target, int offset, int count, float[] data, uint usage)
        {
            IntPtr p = Marshal.AllocHGlobal(count * sizeof(float));
            Marshal.Copy(data, offset, p, count);
            GetDelegateFor<glBufferData>()(target, count * sizeof(float), p, usage);
            Marshal.FreeHGlobal(p);
        }
        public static void BufferData(uint target, float[] data, uint usage)
        {
            IntPtr p = Marshal.AllocHGlobal(data.Length * sizeof(float));
            Marshal.Copy(data, 0, p, data.Length);
            GetDelegateFor<glBufferData>()(target, data.Length * sizeof(float), p, usage);
            Marshal.FreeHGlobal(p);
        }
        public static void BufferData(uint target, ushort[] data, uint usage)
        {
            var dataSize = data.Length * sizeof(ushort);
            IntPtr p = Marshal.AllocHGlobal(dataSize);
            var shortData = new short[data.Length];
            Buffer.BlockCopy(data, 0, shortData, 0, dataSize);
            Marshal.Copy(shortData, 0, p, data.Length);
            GetDelegateFor<glBufferData>()(target, dataSize, p, usage);
            Marshal.FreeHGlobal(p);
        }
        public static void BufferSubData(uint target, int offset, int size, IntPtr data)
        {
            GetDelegateFor<glBufferSubData>()(target, offset, size, data);
        }
        public static void GetBufferSubData(uint target, int offset, int size, IntPtr data)
        {
            GetDelegateFor<glGetBufferSubData>()(target, offset, size, data);
        }
        public static IntPtr MapBuffer(uint target, uint access)
        {
            return (IntPtr)GetDelegateFor<glMapBuffer>()(target, access);
        }
        public static IntPtr MapBufferRange(uint target, int offset, int length, uint access)
        {
            return (IntPtr)GetDelegateFor<glMapBufferRange>()(target, offset, length, access);
        }
        public static bool UnmapBuffer(uint target)
        {
            return (bool)GetDelegateFor<glUnmapBuffer>()(target);
        }
        public static void GetBufferParameter(uint target, uint pname, int[] parameters)
        {
            GetDelegateFor<glGetBufferParameteriv>()(target, pname, parameters);
        }
        public static void GetBufferPointer(uint target, uint pname, IntPtr[] parameters)
        {
            GetDelegateFor<glGetBufferPointerv>()(target, pname, parameters);
        }

        //  Delegates
        private delegate void glGenQueries(int n, uint[] ids);
        private delegate void glDeleteQueries(int n, uint[] ids);
        private delegate bool glIsQuery(uint id);
        private delegate void glBeginQuery(uint target, uint id);
        private delegate void glEndQuery(uint target);
        private delegate void glGetQueryiv(uint target, uint pname, int[] parameters);
        private delegate void glGetQueryObjectiv(uint id, uint pname, int[] parameters);
        private delegate void glGetQueryObjectuiv(uint id, uint pname, uint[] parameters);
        private delegate void glBindBuffer(uint target, uint buffer);
        private delegate void glDeleteBuffers(int n, uint[] buffers);
        private delegate void glGenBuffers(int n, uint[] buffers);
        private delegate bool glIsBuffer(uint buffer);
        private delegate void glBufferData(uint target, int size, IntPtr data, uint usage);
        private delegate void glBufferSubData(uint target, int offset, int size, IntPtr data);
        private delegate void glGetBufferSubData(uint target, int offset, int size, IntPtr data);
        private delegate IntPtr glMapBuffer(uint target, uint access);
        private delegate IntPtr glMapBufferRange(uint target, int offset, int length, uint access);
        private delegate bool glUnmapBuffer(uint target);
        private delegate void glGetBufferParameteriv(uint target, uint pname, int[] parameters);
        private delegate void glGetBufferPointerv(uint target, uint pname, IntPtr[] parameters);

        //  Constants
        public const uint GL_BUFFER_SIZE = 0x8764;
        public const uint GL_BUFFER_USAGE = 0x8765;
        public const uint GL_QUERY_COUNTER_BITS = 0x8864;
        public const uint GL_CURRENT_QUERY = 0x8865;
        public const uint GL_QUERY_RESULT = 0x8866;
        public const uint GL_QUERY_RESULT_AVAILABLE = 0x8867;
        public const uint GL_ARRAY_BUFFER = 0x8892;
        public const uint GL_ELEMENT_ARRAY_BUFFER = 0x8893;
        public const uint GL_ARRAY_BUFFER_BINDING = 0x8894;
        public const uint GL_ELEMENT_ARRAY_BUFFER_BINDING = 0x8895;
        public const uint GL_VERTEX_ATTRIB_ARRAY_BUFFER_BINDING = 0x889F;
        public const uint GL_READ_ONLY = 0x88B8;
        public const uint GL_WRITE_ONLY = 0x88B9;
        public const uint GL_READ_WRITE = 0x88BA;
        public const uint GL_BUFFER_ACCESS = 0x88BB;
        public const uint GL_BUFFER_MAPPED = 0x88BC;
        public const uint GL_BUFFER_MAP_POINTER = 0x88BD;
        public const uint GL_STREAM_DRAW = 0x88E0;
        public const uint GL_STREAM_READ = 0x88E1;
        public const uint GL_STREAM_COPY = 0x88E2;
        public const uint GL_STATIC_DRAW = 0x88E4;
        public const uint GL_STATIC_READ = 0x88E5;
        public const uint GL_STATIC_COPY = 0x88E6;
        public const uint GL_DYNAMIC_DRAW = 0x88E8;
        public const uint GL_DYNAMIC_READ = 0x88E9;
        public const uint GL_DYNAMIC_COPY = 0x88EA;
        public const uint GL_SAMPLES_PASSED = 0x8914;

        public const uint GL_MAP_READ_BIT = 0x0001;
        public const uint GL_MAP_WRITE_BIT = 0x0002;
        public const uint GL_MAP_INVALIDATE_RANGE_BIT = 0x0004;
        public const uint GL_MAP_INVALIDATE_BUFFER_BIT = 0x0008;
        public const uint GL_MAP_FLUSH_EXPLICIT_BIT = 0x0010;
        public const uint GL_MAP_UNSYNCHRONIZED_BIT = 0x0020;

        #endregion

        #region OpenGL 2.0

        //  Methods
        public static void BlendEquationSeparate(uint modeRGB, uint modeAlpha)
        {
            GetDelegateFor<glBlendEquationSeparate>()(modeRGB, modeAlpha);
        }
        public static void DrawBuffers(int n, uint[] bufs)
        {
            GetDelegateFor<glDrawBuffers>()(n, bufs);
        }
        public static void StencilOpSeparate(uint face, uint sfail, uint dpfail, uint dppass)
        {
            GetDelegateFor<glStencilOpSeparate>()(face, sfail, dpfail, dppass);
        }
        public static void StencilFuncSeparate(uint face, uint func, int reference, uint mask)
        {
            GetDelegateFor<glStencilFuncSeparate>()(face, func, reference, mask);
        }
        public static void StencilMaskSeparate(uint face, uint mask)
        {
            GetDelegateFor<glStencilMaskSeparate>()(face, mask);
        }
        public static void AttachShader(uint program, uint shader)
        {
            GetDelegateFor<glAttachShader>()(program, shader);
        }
        public static void BindAttribLocation(uint program, uint index, string name)
        {
            GetDelegateFor<glBindAttribLocation>()(program, index, name);
        }
        /// <summary>
        /// Compile a shader object
        /// </summary>
        /// <param name="shader">Specifies the shader object to be compiled.</param>
        public static void CompileShader(uint shader)
        {
            GetDelegateFor<glCompileShader>()(shader);
        }
        public static uint GenProgramPipeline()
        {
            uint[] pipelines = new uint[1];
            GetDelegateFor<glGenProgramPipelines>()(1, pipelines);
            return pipelines[0];
        }
        public static void BindProgramPipeline(uint pipeline)
        {
            GetDelegateFor<glBindProgramPipeline>()(pipeline);
        }
        public static void DeleteProgramPipeline(uint pipeline)
        {
            GetDelegateFor<glDeleteProgramPipelines>()(1, new[] { pipeline });
        }
        public static uint CreateProgram()
        {
            return (uint)GetDelegateFor<glCreateProgram>()();
        }
        public static uint CreateShaderProgram(uint type, string source)
        {
            return GetDelegateFor<glCreateShaderProgramv>()(type, 1, new[] { source });
        }
        /// <summary>
        /// Create a shader object
        /// </summary>
        /// <param name="type">Specifies the type of shader to be created. Must be either GL_VERTEX_SHADER or GL_FRAGMENT_SHADER.</param>
        /// <returns>This function returns 0 if an error occurs creating the shader object. Otherwise the shader id is returned.</returns>
        public static uint CreateShader(uint type)
        {
            return (uint)GetDelegateFor<glCreateShader>()(type);
        }
        public static void DeleteProgram(uint program)
        {
            GetDelegateFor<glDeleteProgram>()(program);
        }
        public static void DeleteShader(uint shader)
        {
            GetDelegateFor<glDeleteShader>()(shader);
        }
        public static void DetachShader(uint program, uint shader)
        {
            GetDelegateFor<glDetachShader>()(program, shader);
        }
        public static void DisableVertexAttribArray(uint index)
        {
            GetDelegateFor<glDisableVertexAttribArray>()(index);
        }
        public static void EnableVertexAttribArray(uint index)
        {
            GetDelegateFor<glEnableVertexAttribArray>()(index);
        }


        /// <summary>
        /// Return information about an active attribute variable
        /// </summary>
        /// <param name="program">Specifies the program object to be queried.</param>
        /// <param name="index">Specifies the index of the attribute variable to be queried.</param>
        /// <param name="bufSize">Specifies the maximum number of characters OpenGL is allowed to write in the character buffer indicated by <paramref name="name"/>.</param>
        /// <param name="length">Returns the number of characters actually written by OpenGL in the string indicated by name (excluding the null terminator) if a value other than NULL is passed.</param>
        /// <param name="size">Returns the size of the attribute variable.</param>
        /// <param name="type">Returns the data type of the attribute variable.</param>
        /// <param name="name">Returns a null terminated string containing the name of the attribute variable.</param>
        public static void GetActiveAttrib(uint program, uint index, int bufSize, out int length, out int size, out uint type, out string name)
        {
            var builder = new StringBuilder(bufSize);
            GetDelegateFor<glGetActiveAttrib>()(program, index, bufSize, out length, out size, out type, builder);
            name = builder.ToString();
        }

        /// <summary>
        /// Return information about an active uniform variable
        /// </summary>
        /// <param name="program">Specifies the program object to be queried.</param>
        /// <param name="index">Specifies the index of the uniform variable to be queried.</param>
        /// <param name="bufSize">Specifies the maximum number of characters OpenGL is allowed 
        /// to write in the character buffer indicated by <paramref name="name"/>.</param>
        /// <param name="length">Returns the number of characters actually written by OpenGL in the string indicated by name 
        /// (excluding the null terminator) if a value other than NULL is passed.</param>
        /// <param name="size">Returns the size of the uniform variable.</param>
        /// <param name="type">Returns the data type of the uniform variable.</param>
        /// <param name="name">Returns a null terminated string containing the name of the uniform variable.</param>
        public static void GetActiveUniform(uint program, uint index, int bufSize, out int length, out int size, out GLType type, out string name)
        {
            var builder = new StringBuilder(bufSize);
            GetDelegateFor<glGetActiveUniform>()(program, index, bufSize, out length, out size, out uint typeValue, builder);
            type = (GLType)typeValue;
            name = builder.ToString();
        }
        public static void GetActiveUniform(uint program, uint index, out int length, out int size, out GLType type, StringBuilder name)
        {
            GetDelegateFor<glGetActiveUniform>()(program, index, name.Capacity, out length, out size, out uint typeValue, name);
            type = (GLType)typeValue;
        }

        public static void GetAttachedShaders(uint program, int maxCount, int[] count, uint[] obj)
        {
            GetDelegateFor<glGetAttachedShaders>()(program, maxCount, count, obj);
        }
        public static int GetAttribLocation(uint program, string name)
        {
            return (int)GetDelegateFor<glGetAttribLocation>()(program, name);
        }
        public static void GetProgram(uint program, uint pname, int[] parameters)
        {
            GetDelegateFor<glGetProgramiv>()(program, pname, parameters);
        }
        public static int GetProgram(uint program, uint pname)
        {
            int[] parameters = new int[1];
            GetDelegateFor<glGetProgramiv>()(program, pname, parameters);
            return parameters[0];
        }
        private static readonly StringBuilder programLogBuilder = new StringBuilder(2048);
        public static string GetProgramInfoLog(uint program)
        {
            programLogBuilder.Clear();
            programLogBuilder.EnsureCapacity(2048);

            GetProgramInfoLog(program, programLogBuilder.Capacity * sizeof(char), IntPtr.Zero, programLogBuilder);
            return programLogBuilder.ToString();
        }
        public static void GetProgramInfoLog(uint program, int bufSize, IntPtr length, StringBuilder infoLog)
        {
            GetDelegateFor<glGetProgramInfoLog>()(program, bufSize, length, infoLog);
        }
        public static void GetShader(uint shader, uint pname, int[] parameters)
        {
            GetDelegateFor<glGetShaderiv>()(shader, pname, parameters);
        }
        public static int GetShader(uint shader, uint pname)
        {
            int[] parameters = new int[1];
            GetDelegateFor<glGetShaderiv>()(shader, pname, parameters);
            return parameters[0];
        }
        public static void GetShaderInfoLog(uint shader, int bufSize, IntPtr length, StringBuilder infoLog)
        {
            GetDelegateFor<glGetShaderInfoLog>()(shader, bufSize, length, infoLog);
        }
        public static string GetShaderInfoLog(uint shader)
        {
            int bufSize = GetShader(shader, GL_INFO_LOG_LENGTH);
            var infoLog = new StringBuilder(bufSize);
            GetDelegateFor<glGetShaderInfoLog>()(shader, bufSize, IntPtr.Zero, infoLog);
            return infoLog.ToString();
        }
        public static void GetShaderSource(uint shader, int bufSize, IntPtr length, StringBuilder source)
        {
            GetDelegateFor<glGetShaderSource>()(shader, bufSize, length, source);
        }
        /// <summary>
        /// Returns an integer that represents the location of a specific uniform variable within a program object. name must be a null terminated string that contains no white space. name must be an active uniform variable name in program that is not a structure, an array of structures, or a subcomponent of a vector or a matrix. This function returns -1 if name does not correspond to an active uniform variable in program, if name starts with the reserved prefix "gl_", or if name is associated with an atomic counter or a named uniform block.
        /// </summary>
        /// <param name="program">Specifies the program object to be queried.</param>
        /// <param name="name">Points to a null terminated string containing the name of the uniform variable whose location is to be queried.</param>
        /// <returns></returns>
        public static int GetUniformLocation(uint program, string name)
        {
            return (int)GetDelegateFor<glGetUniformLocation>()(program, name);
        }
        public static void GetUniform(uint program, int location, float[] parameters)
        {
            GetDelegateFor<glGetUniformfv>()(program, location, parameters);
        }
        public static float[] GetUniformf(uint program, int location, int count)
        {
            float[] parameters = new float[count];
            GetDelegateFor<glGetUniformfv>()(program, location, parameters);
            return parameters;
        }
        public static void GetUniform(uint program, int location, int[] parameters)
        {
            GetDelegateFor<glGetUniformiv>()(program, location, parameters);
        }
        public static int[] GetUniformi(uint program, int location, int count)
        {
            int[] parameters = new int[count];
            GetDelegateFor<glGetUniformiv>()(program, location, parameters);
            return parameters;
        }
        public static void GetVertexAttrib(uint index, uint pname, double[] parameters)
        {
            GetDelegateFor<glGetVertexAttribdv>()(index, pname, parameters);
        }
        public static void GetVertexAttrib(uint index, uint pname, float[] parameters)
        {
            GetDelegateFor<glGetVertexAttribfv>()(index, pname, parameters);
        }
        public static void GetVertexAttrib(uint index, uint pname, int[] parameters)
        {
            GetDelegateFor<glGetVertexAttribiv>()(index, pname, parameters);
        }
        public static void GetVertexAttribPointer(uint index, uint pname, IntPtr pointer)
        {
            GetDelegateFor<glGetVertexAttribPointerv>()(index, pname, pointer);
        }
        public static bool IsProgram(uint program)
        {
            return (bool)GetDelegateFor<glIsProgram>()(program);
        }
        public static bool IsShader(uint shader)
        {
            return (bool)GetDelegateFor<glIsShader>()(shader);
        }
        public static void LinkProgram(uint program)
        {
            GetDelegateFor<glLinkProgram>()(program);
        }

        /// <summary>
        /// Replace the source code in a shader object
        /// </summary>
        /// <param name="shader">Specifies the handle of the shader object whose source code is to be replaced.</param>
        /// <param name="source">The source.</param>
        public static void ShaderSource(uint shader, string source)
        {
            //  Remember, the function takes an array of strings but concatenates them, so we should NOT split into lines!
            GetDelegateFor<glShaderSource>()(shader, 1, new[] { source }, new[] { source.Length });
        }

        public static IntPtr StringToPtrAnsi(string str)
        {
            if (string.IsNullOrEmpty(str))
                return IntPtr.Zero;

            byte[] bytes = Encoding.ASCII.GetBytes(str + '\0');
            IntPtr strPtr = Marshal.AllocHGlobal(bytes.Length);
            Marshal.Copy(bytes, 0, strPtr, bytes.Length);

            return strPtr;
        }
        public static void UseProgram(uint program)
        {
            GetDelegateFor<glUseProgram>()(program);
        }
        public static void UseProgramStages(uint pipeline, uint stage, uint program)
        {
            GetDelegateFor<glUseProgramStages>()(pipeline, stage, program);
        }
        public static void Uniform1(int location, float v0)
        {
            GetDelegateFor<glUniform1f>()(location, v0);
        }
        public static void Uniform2(int location, float v0, float v1)
        {
            GetDelegateFor<glUniform2f>()(location, v0, v1);
        }
        public static void Uniform3(int location, float v0, float v1, float v2)
        {
            GetDelegateFor<glUniform3f>()(location, v0, v1, v2);
        }
        public static void Uniform4(int location, float v0, float v1, float v2, float v3)
        {
            GetDelegateFor<glUniform4f>()(location, v0, v1, v2, v3);
        }
        public static void Uniform1(int location, int v0)
        {
            GetDelegateFor<glUniform1i>()(location, v0);
        }
        public static void Uniform2(int location, int v0, int v1)
        {
            GetDelegateFor<glUniform2i>()(location, v0, v1);
        }
        public static void Uniform3(int location, int v0, int v1, int v2)
        {
            GetDelegateFor<glUniform3i>()(location, v0, v1, v2);
        }
        public static void Uniform(int location, int v0, int v1, int v2, int v3)
        {
            GetDelegateFor<glUniform4i>()(location, v0, v1, v2, v3);
        }
        public static void Uniform1(int location, int count, float[] value)
        {
            GetDelegateFor<glUniform1fv>()(location, count, value);
        }
        public static void Uniform2(int location, int count, float[] value)
        {
            GetDelegateFor<glUniform2fv>()(location, count, value);
        }
        public static void Uniform3(int location, int count, float[] value)
        {
            GetDelegateFor<glUniform3fv>()(location, count, value);
        }
        public static void Uniform4(int location, int count, float[] value)
        {
            GetDelegateFor<glUniform4fv>()(location, count, value);
        }
        public static void Uniform1(int location, int count, int[] value)
        {
            GetDelegateFor<glUniform1iv>()(location, count, value);
        }
        public static void Uniform2(int location, int count, int[] value)
        {
            GetDelegateFor<glUniform2iv>()(location, count, value);
        }
        public static void Uniform3(int location, int count, int[] value)
        {
            GetDelegateFor<glUniform3iv>()(location, count, value);
        }
        public static void Uniform4(int location, int count, int[] value)
        {
            GetDelegateFor<glUniform4iv>()(location, count, value);
        }
        public static void UniformMatrix2(int location, int count, bool transpose, float[] value)
        {
            GetDelegateFor<glUniformMatrix2fv>()(location, count, transpose, value);
        }
        public static void UniformMatrix3(int location, int count, bool transpose, float[] value)
        {
            GetDelegateFor<glUniformMatrix3fv>()(location, count, transpose, value);
        }
        public static void UniformMatrix4(int location, int count, bool transpose, float[] value)
        {
            GetDelegateFor<glUniformMatrix4fv>()(location, count, transpose, value);
        }
        public static void ProgramUniform1(uint program, int location, float v0)
        {
            GetDelegateFor<glProgramUniform1f>()(program, location, v0);
        }
        public static void ProgramUniform2(uint program, int location, float v0, float v1)
        {
            GetDelegateFor<glProgramUniform2f>()(program, location, v0, v1);
        }
        public static void ProgramUniform3(uint program, int location, float v0, float v1, float v2)
        {
            GetDelegateFor<glProgramUniform3f>()(program, location, v0, v1, v2);
        }
        public static void ProgramUniform4(uint program, int location, float v0, float v1, float v2, float v3)
        {
            GetDelegateFor<glProgramUniform4f>()(program, location, v0, v1, v2, v3);
        }
        public static void ProgramUniform1(uint program, int location, int v0)
        {
            GetDelegateFor<glProgramUniform1i>()(program, location, v0);
        }
        public static void ProgramUniform2(uint program, int location, int v0, int v1)
        {
            GetDelegateFor<glProgramUniform2i>()(program, location, v0, v1);
        }
        public static void ProgramUniform3(uint program, int location, int v0, int v1, int v2)
        {
            GetDelegateFor<glProgramUniform3i>()(program, location, v0, v1, v2);
        }
        public static void ProgramUniform(uint program, int location, int v0, int v1, int v2, int v3)
        {
            GetDelegateFor<glProgramUniform4i>()(program, location, v0, v1, v2, v3);
        }
        public static void ProgramUniform1(uint program, int location, int count, float[] value)
        {
            GetDelegateFor<glProgramUniform1fv>()(program, location, count, value);
        }
        public static void ProgramUniform2(uint program, int location, int count, float[] value)
        {
            GetDelegateFor<glProgramUniform2fv>()(program, location, count, value);
        }
        public static void ProgramUniform3(uint program, int location, int count, float[] value)
        {
            GetDelegateFor<glProgramUniform3fv>()(program, location, count, value);
        }
        public static void ProgramUniform4(uint program, int location, int count, float[] value)
        {
            GetDelegateFor<glProgramUniform4fv>()(program, location, count, value);
        }
        public static void ProgramUniform1(uint program, int location, int count, int[] value)
        {
            GetDelegateFor<glProgramUniform1iv>()(program, location, count, value);
        }
        public static void ProgramUniform2(uint program, int location, int count, int[] value)
        {
            GetDelegateFor<glProgramUniform2iv>()(program, location, count, value);
        }
        public static void ProgramUniform3(uint program, int location, int count, int[] value)
        {
            GetDelegateFor<glProgramUniform3iv>()(program, location, count, value);
        }
        public static void ProgramUniform4(uint program, int location, int count, int[] value)
        {
            GetDelegateFor<glProgramUniform4iv>()(program, location, count, value);
        }
        public static void ProgramUniformMatrix2(uint program, int location, int count, bool transpose, float[] value)
        {
            GetDelegateFor<glProgramUniformMatrix2fv>()(program, location, count, transpose, value);
        }
        public static void ProgramUniformMatrix3(uint program, int location, int count, bool transpose, float[] value)
        {
            GetDelegateFor<glProgramUniformMatrix3fv>()(program, location, count, transpose, value);
        }
        public static void ProgramUniformMatrix4(uint program, int location, int count, bool transpose, float[] value)
        {
            GetDelegateFor<glProgramUniformMatrix4fv>()(program, location, count, transpose, value);
        }
        public static void ValidateProgram(uint program)
        {
            GetDelegateFor<glValidateProgram>()(program);
        }
        public static void VertexAttrib1(uint index, double x)
        {
            GetDelegateFor<glVertexAttrib1d>()(index, x);
        }
        public static void VertexAttrib1(uint index, double[] v)
        {
            GetDelegateFor<glVertexAttrib1dv>()(index, v);
        }
        public static void VertexAttrib(uint index, float x)
        {
            GetDelegateFor<glVertexAttrib1f>()(index, x);
        }
        public static void VertexAttrib1(uint index, float[] v)
        {
            GetDelegateFor<glVertexAttrib1fv>()(index, v);
        }
        public static void VertexAttrib(uint index, short x)
        {
            GetDelegateFor<glVertexAttrib1s>()(index, x);
        }
        public static void VertexAttrib1(uint index, short[] v)
        {
            GetDelegateFor<glVertexAttrib1sv>()(index, v);
        }
        public static void VertexAttrib2(uint index, double x, double y)
        {
            GetDelegateFor<glVertexAttrib2d>()(index, x, y);
        }
        public static void VertexAttrib2(uint index, double[] v)
        {
            GetDelegateFor<glVertexAttrib2dv>()(index, v);
        }
        public static void VertexAttrib2(uint index, float x, float y)
        {
            GetDelegateFor<glVertexAttrib2f>()(index, x, y);
        }
        public static void VertexAttrib2(uint index, float[] v)
        {
            GetDelegateFor<glVertexAttrib2fv>()(index, v);
        }
        public static void VertexAttrib2(uint index, short x, short y)
        {
            GetDelegateFor<glVertexAttrib2s>()(index, x, y);
        }
        public static void VertexAttrib2(uint index, short[] v)
        {
            GetDelegateFor<glVertexAttrib2sv>()(index, v);
        }
        public static void VertexAttrib3(uint index, double x, double y, double z)
        {
            GetDelegateFor<glVertexAttrib3d>()(index, x, y, z);
        }
        public static void VertexAttrib3(uint index, double[] v)
        {
            GetDelegateFor<glVertexAttrib3dv>()(index, v);
        }
        public static void VertexAttrib3(uint index, float x, float y, float z)
        {
            GetDelegateFor<glVertexAttrib3f>()(index, x, y, z);
        }
        public static void VertexAttrib3(uint index, float[] v)
        {
            GetDelegateFor<glVertexAttrib3fv>()(index, v);
        }
        public static void VertexAttrib3(uint index, short x, short y, short z)
        {
            GetDelegateFor<glVertexAttrib3s>()(index, x, y, z);
        }
        public static void VertexAttrib3(uint index, short[] v)
        {
            GetDelegateFor<glVertexAttrib3sv>()(index, v);
        }
        public static void VertexAttrib4N(uint index, sbyte[] v)
        {
            GetDelegateFor<glVertexAttrib4Nbv>()(index, v);
        }
        public static void VertexAttrib4N(uint index, int[] v)
        {
            GetDelegateFor<glVertexAttrib4Niv>()(index, v);
        }
        public static void VertexAttrib4N(uint index, short[] v)
        {
            GetDelegateFor<glVertexAttrib4Nsv>()(index, v);
        }
        public static void VertexAttrib4N(uint index, byte x, byte y, byte z, byte w)
        {
            GetDelegateFor<glVertexAttrib4Nub>()(index, x, y, z, w);
        }
        public static void VertexAttrib4N(uint index, byte[] v)
        {
            GetDelegateFor<glVertexAttrib4Nubv>()(index, v);
        }
        public static void VertexAttrib4N(uint index, uint[] v)
        {
            GetDelegateFor<glVertexAttrib4Nuiv>()(index, v);
        }
        public static void VertexAttrib4N(uint index, ushort[] v)
        {
            GetDelegateFor<glVertexAttrib4Nusv>()(index, v);
        }
        public static void VertexAttrib4(uint index, sbyte[] v)
        {
            GetDelegateFor<glVertexAttrib4bv>()(index, v);
        }
        public static void VertexAttrib4(uint index, double x, double y, double z, double w)
        {
            GetDelegateFor<glVertexAttrib4d>()(index, x, y, z, w);
        }
        public static void VertexAttrib4(uint index, double[] v)
        {
            GetDelegateFor<glVertexAttrib4dv>()(index, v);
        }
        public static void VertexAttrib4(uint index, float x, float y, float z, float w)
        {
            GetDelegateFor<glVertexAttrib4f>()(index, x, y, z, w);
        }
        public static void VertexAttrib4(uint index, float[] v)
        {
            GetDelegateFor<glVertexAttrib4fv>()(index, v);
        }
        public static void VertexAttrib4(uint index, int[] v)
        {
            GetDelegateFor<glVertexAttrib4iv>()(index, v);
        }
        public static void VertexAttrib4(uint index, short x, short y, short z, short w)
        {
            GetDelegateFor<glVertexAttrib4s>()(index, x, y, z, w);
        }
        public static void VertexAttrib4(uint index, short[] v)
        {
            GetDelegateFor<glVertexAttrib4sv>()(index, v);
        }
        public static void VertexAttrib4(uint index, byte[] v)
        {
            GetDelegateFor<glVertexAttrib4ubv>()(index, v);
        }
        public static void VertexAttrib4(uint index, uint[] v)
        {
            GetDelegateFor<glVertexAttrib4uiv>()(index, v);
        }
        public static void VertexAttrib4(uint index, ushort[] v)
        {
            GetDelegateFor<glVertexAttrib4usv>()(index, v);
        }
        public static void VertexAttribPointer(uint index, int size, uint type, bool normalized, int stride, IntPtr pointer)
        {
            GetDelegateFor<glVertexAttribPointer>()(index, size, type, normalized, stride, pointer);
        }

        //  Delegates
        private delegate void glBlendEquationSeparate(uint modeRGB, uint modeAlpha);
        private delegate void glDrawBuffers(int n, uint[] bufs);
        private delegate void glStencilOpSeparate(uint face, uint sfail, uint dpfail, uint dppass);
        private delegate void glStencilFuncSeparate(uint face, uint func, int reference, uint mask);
        private delegate void glStencilMaskSeparate(uint face, uint mask);
        private delegate void glAttachShader(uint program, uint shader);
        private delegate void glBindAttribLocation(uint program, uint index, string name);
        private delegate void glCompileShader(uint shader);
        private delegate void glGenProgramPipelines(int n, uint[] pipelines);
        private delegate void glDeleteProgramPipelines(int n, uint[] pipelines);
        private delegate void glBindProgramPipeline(uint pipeline);
        private delegate uint glCreateProgram();
        //  By specifying 'ThrowOnUnmappableChar' we protect ourselves from inadvertantly using a unicode character
        //  in the source which the marshaller cannot map. Without this, it maps it to '?' leading to long and pointless
        //  sessions of trying to find bugs in the shader, which are most often just copied and pasted unicode characters!
        //  If you're getting exceptions here, remove all unicode crap from your input files (remember, some unicode 
        //  characters you can't even see).
        [UnmanagedFunctionPointer(CallingConvention.StdCall, ThrowOnUnmappableChar = true)]
        private delegate uint glCreateShaderProgramv(uint type, uint count, string[] sources);
        private delegate uint glCreateShader(uint type);
        private delegate void glDeleteProgram(uint program);
        private delegate void glDeleteShader(uint shader);
        private delegate void glDetachShader(uint program, uint shader);
        private delegate void glDisableVertexAttribArray(uint index);
        private delegate void glEnableVertexAttribArray(uint index);
        private delegate void glGetActiveAttrib(uint program, uint index, int bufSize, out int length, out int size, out uint type, StringBuilder name);
        private delegate void glGetActiveUniform(uint program, uint index, int bufSize, out int length, out int size, out uint type, StringBuilder name);
        private delegate void glGetAttachedShaders(uint program, int maxCount, int[] count, uint[] obj);
        private delegate int glGetAttribLocation(uint program, string name);
        private delegate void glGetProgramiv(uint program, uint pname, int[] parameters);
        private delegate void glGetProgramInfoLog(uint program, int bufSize, IntPtr length, StringBuilder infoLog);
        private delegate void glGetShaderiv(uint shader, uint pname, int[] parameters);
        private delegate void glGetShaderInfoLog(uint shader, int bufSize, IntPtr length, StringBuilder infoLog);
        private delegate void glGetShaderSource(uint shader, int bufSize, IntPtr length, StringBuilder source);
        private delegate int glGetUniformLocation(uint program, string name);
        private delegate void glGetUniformfv(uint program, int location, float[] parameters);
        private delegate void glGetUniformiv(uint program, int location, int[] parameters);
        private delegate void glGetVertexAttribdv(uint index, uint pname, double[] parameters);
        private delegate void glGetVertexAttribfv(uint index, uint pname, float[] parameters);
        private delegate void glGetVertexAttribiv(uint index, uint pname, int[] parameters);
        private delegate void glGetVertexAttribPointerv(uint index, uint pname, IntPtr pointer);
        private delegate bool glIsProgram(uint program);
        private delegate bool glIsShader(uint shader);
        private delegate void glLinkProgram(uint program);
        //  By specifying 'ThrowOnUnmappableChar' we protect ourselves from inadvertantly using a unicode character
        //  in the source which the marshaller cannot map. Without this, it maps it to '?' leading to long and pointless
        //  sessions of trying to find bugs in the shader, which are most often just copied and pasted unicode characters!
        //  If you're getting exceptions here, remove all unicode crap from your input files (remember, some unicode 
        //  characters you can't even see).
        [UnmanagedFunctionPointer(CallingConvention.StdCall, ThrowOnUnmappableChar = true)]
        private delegate void glShaderSource(uint shader, int count, string[] source, int[] length);
        private delegate void glUseProgram(uint program);
        private delegate void glUseProgramStages(uint pipeline, uint stage, uint program);
        private delegate void glUniform1f(int location, float v0);
        private delegate void glUniform2f(int location, float v0, float v1);
        private delegate void glUniform3f(int location, float v0, float v1, float v2);
        private delegate void glUniform4f(int location, float v0, float v1, float v2, float v3);
        private delegate void glUniform1i(int location, int v0);
        private delegate void glUniform2i(int location, int v0, int v1);
        private delegate void glUniform3i(int location, int v0, int v1, int v2);
        private delegate void glUniform4i(int location, int v0, int v1, int v2, int v3);
        private delegate void glUniform1fv(int location, int count, float[] value);
        private delegate void glUniform2fv(int location, int count, float[] value);
        private delegate void glUniform3fv(int location, int count, float[] value);
        private delegate void glUniform4fv(int location, int count, float[] value);
        private delegate void glUniform1iv(int location, int count, int[] value);
        private delegate void glUniform2iv(int location, int count, int[] value);
        private delegate void glUniform3iv(int location, int count, int[] value);
        private delegate void glUniform4iv(int location, int count, int[] value);
        private delegate void glUniformMatrix2fv(int location, int count, bool transpose, float[] value);
        private delegate void glUniformMatrix3fv(int location, int count, bool transpose, float[] value);
        private delegate void glUniformMatrix4fv(int location, int count, bool transpose, float[] value);
        private delegate void glProgramUniform1f(uint program, int location, float v0);
        private delegate void glProgramUniform2f(uint program, int location, float v0, float v1);
        private delegate void glProgramUniform3f(uint program, int location, float v0, float v1, float v2);
        private delegate void glProgramUniform4f(uint program, int location, float v0, float v1, float v2, float v3);
        private delegate void glProgramUniform1i(uint program, int location, int v0);
        private delegate void glProgramUniform2i(uint program, int location, int v0, int v1);
        private delegate void glProgramUniform3i(uint program, int location, int v0, int v1, int v2);
        private delegate void glProgramUniform4i(uint program, int location, int v0, int v1, int v2, int v3);
        private delegate void glProgramUniform1fv(uint program, int location, int count, float[] value);
        private delegate void glProgramUniform2fv(uint program, int location, int count, float[] value);
        private delegate void glProgramUniform3fv(uint program, int location, int count, float[] value);
        private delegate void glProgramUniform4fv(uint program, int location, int count, float[] value);
        private delegate void glProgramUniform1iv(uint program, int location, int count, int[] value);
        private delegate void glProgramUniform2iv(uint program, int location, int count, int[] value);
        private delegate void glProgramUniform3iv(uint program, int location, int count, int[] value);
        private delegate void glProgramUniform4iv(uint program, int location, int count, int[] value);
        private delegate void glProgramUniformMatrix2fv(uint program, int location, int count, bool transpose, float[] value);
        private delegate void glProgramUniformMatrix3fv(uint program, int location, int count, bool transpose, float[] value);
        private delegate void glProgramUniformMatrix4fv(uint program, int location, int count, bool transpose, float[] value);
        private delegate void glValidateProgram(uint program);
        private delegate void glVertexAttrib1d(uint index, double x);
        private delegate void glVertexAttrib1dv(uint index, double[] v);
        private delegate void glVertexAttrib1f(uint index, float x);
        private delegate void glVertexAttrib1fv(uint index, float[] v);
        private delegate void glVertexAttrib1s(uint index, short x);
        private delegate void glVertexAttrib1sv(uint index, short[] v);
        private delegate void glVertexAttrib2d(uint index, double x, double y);
        private delegate void glVertexAttrib2dv(uint index, double[] v);
        private delegate void glVertexAttrib2f(uint index, float x, float y);
        private delegate void glVertexAttrib2fv(uint index, float[] v);
        private delegate void glVertexAttrib2s(uint index, short x, short y);
        private delegate void glVertexAttrib2sv(uint index, short[] v);
        private delegate void glVertexAttrib3d(uint index, double x, double y, double z);
        private delegate void glVertexAttrib3dv(uint index, double[] v);
        private delegate void glVertexAttrib3f(uint index, float x, float y, float z);
        private delegate void glVertexAttrib3fv(uint index, float[] v);
        private delegate void glVertexAttrib3s(uint index, short x, short y, short z);
        private delegate void glVertexAttrib3sv(uint index, short[] v);
        private delegate void glVertexAttrib4Nbv(uint index, sbyte[] v);
        private delegate void glVertexAttrib4Niv(uint index, int[] v);
        private delegate void glVertexAttrib4Nsv(uint index, short[] v);
        private delegate void glVertexAttrib4Nub(uint index, byte x, byte y, byte z, byte w);
        private delegate void glVertexAttrib4Nubv(uint index, byte[] v);
        private delegate void glVertexAttrib4Nuiv(uint index, uint[] v);
        private delegate void glVertexAttrib4Nusv(uint index, ushort[] v);
        private delegate void glVertexAttrib4bv(uint index, sbyte[] v);
        private delegate void glVertexAttrib4d(uint index, double x, double y, double z, double w);
        private delegate void glVertexAttrib4dv(uint index, double[] v);
        private delegate void glVertexAttrib4f(uint index, float x, float y, float z, float w);
        private delegate void glVertexAttrib4fv(uint index, float[] v);
        private delegate void glVertexAttrib4iv(uint index, int[] v);
        private delegate void glVertexAttrib4s(uint index, short x, short y, short z, short w);
        private delegate void glVertexAttrib4sv(uint index, short[] v);
        private delegate void glVertexAttrib4ubv(uint index, byte[] v);
        private delegate void glVertexAttrib4uiv(uint index, uint[] v);
        private delegate void glVertexAttrib4usv(uint index, ushort[] v);
        private delegate void glVertexAttribPointer(uint index, int size, uint type, bool normalized, int stride, IntPtr pointer);

        //  Constants
        public const uint GL_BLEND_EQUATION_RGB = 0x8009;
        public const uint GL_VERTEX_ATTRIB_ARRAY_ENABLED = 0x8622;
        public const uint GL_VERTEX_ATTRIB_ARRAY_SIZE = 0x8623;
        public const uint GL_VERTEX_ATTRIB_ARRAY_STRIDE = 0x8624;
        public const uint GL_VERTEX_ATTRIB_ARRAY_TYPE = 0x8625;
        public const uint GL_CURRENT_VERTEX_ATTRIB = 0x8626;
        public const uint GL_VERTEX_PROGRAM_POINT_SIZE = 0x8642;
        public const uint GL_VERTEX_ATTRIB_ARRAY_POINTER = 0x8645;
        public const uint GL_STENCIL_BACK_FUNC = 0x8800;
        public const uint GL_STENCIL_BACK_FAIL = 0x8801;
        public const uint GL_STENCIL_BACK_PASS_DEPTH_FAIL = 0x8802;
        public const uint GL_STENCIL_BACK_PASS_DEPTH_PASS = 0x8803;
        public const uint GL_MAX_DRAW_BUFFERS = 0x8824;
        public const uint GL_DRAW_BUFFER0 = 0x8825;
        public const uint GL_DRAW_BUFFER1 = 0x8826;
        public const uint GL_DRAW_BUFFER2 = 0x8827;
        public const uint GL_DRAW_BUFFER3 = 0x8828;
        public const uint GL_DRAW_BUFFER4 = 0x8829;
        public const uint GL_DRAW_BUFFER5 = 0x882A;
        public const uint GL_DRAW_BUFFER6 = 0x882B;
        public const uint GL_DRAW_BUFFER7 = 0x882C;
        public const uint GL_DRAW_BUFFER8 = 0x882D;
        public const uint GL_DRAW_BUFFER9 = 0x882E;
        public const uint GL_DRAW_BUFFER10 = 0x882F;
        public const uint GL_DRAW_BUFFER11 = 0x8830;
        public const uint GL_DRAW_BUFFER12 = 0x8831;
        public const uint GL_DRAW_BUFFER13 = 0x8832;
        public const uint GL_DRAW_BUFFER14 = 0x8833;
        public const uint GL_DRAW_BUFFER15 = 0x8834;
        public const uint GL_BLEND_EQUATION_ALPHA = 0x883D;
        public const uint GL_MAX_VERTEX_ATTRIBS = 0x8869;
        public const uint GL_VERTEX_ATTRIB_ARRAY_NORMALIZED = 0x886A;
        public const uint GL_MAX_TEXTURE_IMAGE_UNITS = 0x8872;
        public const uint GL_FRAGMENT_SHADER = 0x8B30;
        public const uint GL_FRAGMENT_SHADER_BIT = 0x02;
        public const uint GL_VERTEX_SHADER = 0x8B31;
        public const uint GL_VERTEX_SHADER_BIT = 0x01;
        public const uint GL_MAX_FRAGMENT_UNIFORM_COMPONENTS = 0x8B49;
        public const uint GL_MAX_VERTEX_UNIFORM_COMPONENTS = 0x8B4A;
        public const uint GL_MAX_VARYING_FLOATS = 0x8B4B;
        public const uint GL_MAX_VERTEX_TEXTURE_IMAGE_UNITS = 0x8B4C;
        public const uint GL_MAX_COMBINED_TEXTURE_IMAGE_UNITS = 0x8B4D;
        public const uint GL_SHADER_TYPE = 0x8B4F;
        public const uint GL_FLOAT_VEC2 = 0x8B50;
        public const uint GL_FLOAT_VEC3 = 0x8B51;
        public const uint GL_FLOAT_VEC4 = 0x8B52;
        public const uint GL_INT_VEC2 = 0x8B53;
        public const uint GL_INT_VEC3 = 0x8B54;
        public const uint GL_INT_VEC4 = 0x8B55;
        public const uint GL_BOOL = 0x8B56;
        public const uint GL_BOOL_VEC2 = 0x8B57;
        public const uint GL_BOOL_VEC3 = 0x8B58;
        public const uint GL_BOOL_VEC4 = 0x8B59;
        public const uint GL_FLOAT_MAT2 = 0x8B5A;
        public const uint GL_FLOAT_MAT3 = 0x8B5B;
        public const uint GL_FLOAT_MAT4 = 0x8B5C;
        public const uint GL_SAMPLER_1D = 0x8B5D;
        public const uint GL_SAMPLER_2D = 0x8B5E;
        public const uint GL_SAMPLER_3D = 0x8B5F;
        public const uint GL_SAMPLER_CUBE = 0x8B60;
        public const uint GL_SAMPLER_1D_SHADOW = 0x8B61;
        public const uint GL_SAMPLER_2D_SHADOW = 0x8B62;
        public const uint GL_DELETE_STATUS = 0x8B80;
        public const uint GL_COMPILE_STATUS = 0x8B81;
        public const uint GL_LINK_STATUS = 0x8B82;
        public const uint GL_VALIDATE_STATUS = 0x8B83;
        public const uint GL_INFO_LOG_LENGTH = 0x8B84;
        public const uint GL_ATTACHED_SHADERS = 0x8B85;
        public const uint GL_ACTIVE_UNIFORMS = 0x8B86;
        public const uint GL_ACTIVE_UNIFORM_MAX_LENGTH = 0x8B87;
        public const uint GL_SHADER_SOURCE_LENGTH = 0x8B88;
        public const uint GL_ACTIVE_ATTRIBUTES = 0x8B89;
        public const uint GL_ACTIVE_ATTRIBUTE_MAX_LENGTH = 0x8B8A;
        public const uint GL_FRAGMENT_SHADER_DERIVATIVE_HINT = 0x8B8B;
        public const uint GL_SHADING_LANGUAGE_VERSION = 0x8B8C;
        public const uint GL_CURRENT_PROGRAM = 0x8B8D;
        public const uint GL_POINT_SPRITE_COORD_ORIGIN = 0x8CA0;
        public const uint GL_LOWER_LEFT = 0x8CA1;
        public const uint GL_UPPER_LEFT = 0x8CA2;
        public const uint GL_STENCIL_BACK_REF = 0x8CA3;
        public const uint GL_STENCIL_BACK_VALUE_MASK = 0x8CA4;
        public const uint GL_STENCIL_BACK_WRITEMASK = 0x8CA5;

        #endregion

        #region OpenGL 2.1

        //  Methods
        public static void UniformMatrix2x3(int location, int count, bool transpose, float[] value)
        {
            GetDelegateFor<glUniformMatrix2x3fv>()(location, count, transpose, value);
        }
        public static void UniformMatrix3x2(int location, int count, bool transpose, float[] value)
        {
            GetDelegateFor<glUniformMatrix3x2fv>()(location, count, transpose, value);
        }
        public static void UniformMatrix2x4(int location, int count, bool transpose, float[] value)
        {
            GetDelegateFor<glUniformMatrix2x4fv>()(location, count, transpose, value);
        }
        public static void UniformMatrix4x2(int location, int count, bool transpose, float[] value)
        {
            GetDelegateFor<glUniformMatrix4x2fv>()(location, count, transpose, value);
        }
        public static void UniformMatrix3x4(int location, int count, bool transpose, float[] value)
        {
            GetDelegateFor<glUniformMatrix3x4fv>()(location, count, transpose, value);
        }
        public static void UniformMatrix4x3(int location, int count, bool transpose, float[] value)
        {
            GetDelegateFor<glUniformMatrix4x3fv>()(location, count, transpose, value);
        }

        //  Delegates
        private delegate void glUniformMatrix2x3fv(int location, int count, bool transpose, float[] value);
        private delegate void glUniformMatrix3x2fv(int location, int count, bool transpose, float[] value);
        private delegate void glUniformMatrix2x4fv(int location, int count, bool transpose, float[] value);
        private delegate void glUniformMatrix4x2fv(int location, int count, bool transpose, float[] value);
        private delegate void glUniformMatrix3x4fv(int location, int count, bool transpose, float[] value);
        private delegate void glUniformMatrix4x3fv(int location, int count, bool transpose, float[] value);

        //  Constants
        public const uint GL_PIXEL_PACK_BUFFER = 0x88EB;
        public const uint GL_PIXEL_UNPACK_BUFFER = 0x88EC;
        public const uint GL_PIXEL_PACK_BUFFER_BINDING = 0x88ED;
        public const uint GL_PIXEL_UNPACK_BUFFER_BINDING = 0x88EF;
        public const uint GL_FLOAT_MAT2x3 = 0x8B65;
        public const uint GL_FLOAT_MAT2x4 = 0x8B66;
        public const uint GL_FLOAT_MAT3x2 = 0x8B67;
        public const uint GL_FLOAT_MAT3x4 = 0x8B68;
        public const uint GL_FLOAT_MAT4x2 = 0x8B69;
        public const uint GL_FLOAT_MAT4x3 = 0x8B6A;
        public const uint GL_SRGB = 0x8C40;
        public const uint GL_SRGB8 = 0x8C41;
        public const uint GL_SRGB_ALPHA = 0x8C42;
        public const uint GL_SRGB8_ALPHA8 = 0x8C43;
        public const uint GL_COMPRESSED_SRGB = 0x8C48;
        public const uint GL_COMPRESSED_SRGB_ALPHA = 0x8C49;

        #endregion

        #region OpenGL 3.0

        //  Methods
        public static void ColorMask(uint index, bool r, bool g, bool b, bool a)
        {
            GetDelegateFor<glColorMaski>()(index, r, g, b, a);
        }
        public static void GetBoolean(uint target, uint index, bool[] data)
        {
            GetDelegateFor<glGetBooleani_v>()(target, index, data);
        }
        public static void GetInteger(uint target, uint index, int[] data)
        {
            GetDelegateFor<glGetIntegeri_v>()(target, index, data);
        }
        public static void Enable(uint target, uint index)
        {
            GetDelegateFor<glEnablei>()(target, index);
        }
        public static void Disable(uint target, uint index)
        {
            GetDelegateFor<glDisablei>()(target, index);
        }
        public static bool IsEnabled(uint target, uint index)
        {
            return (bool)GetDelegateFor<glIsEnabledi>()(target, index);
        }
        public static void BeginTransformFeedback(uint primitiveMode)
        {
            GetDelegateFor<glBeginTransformFeedback>()(primitiveMode);
        }
        public static void EndTransformFeedback()
        {
            GetDelegateFor<glEndTransformFeedback>()();
        }
        public static void BindBufferRange(uint target, uint index, uint buffer, int offset, int size)
        {
            GetDelegateFor<glBindBufferRange>()(target, index, buffer, offset, size);
        }
        public static void BindBufferBase(uint target, uint index, uint buffer)
        {
            GetDelegateFor<glBindBufferBase>()(target, index, buffer);
        }
        public static void TransformFeedbackVaryings(uint program, int count, string[] varyings, uint bufferMode)
        {
            GetDelegateFor<glTransformFeedbackVaryings>()(program, count, varyings, bufferMode);
        }
        public static void GetTransformFeedbackVarying(uint program, uint index, int bufSize, int[] length, int[] size, uint[] type, string name)
        {
            GetDelegateFor<glGetTransformFeedbackVarying>()(program, index, bufSize, length, size, type, name);
        }
        public static void ClampColor(uint target, uint clamp)
        {
            GetDelegateFor<glClampColor>()(target, clamp);
        }
        public static void BeginConditionalRender(uint id, uint mode)
        {
            GetDelegateFor<glBeginConditionalRender>()(id, mode);
        }
        public static void EndConditionalRender()
        {
            GetDelegateFor<glEndConditionalRender>()();
        }
        public static void VertexAttribIPointer(uint index, int size, uint type, int stride, IntPtr pointer)
        {
            GetDelegateFor<glVertexAttribIPointer>()(index, size, type, stride, pointer);
        }
        public static void GetVertexAttribI(uint index, uint pname, int[] parameters)
        {
            GetDelegateFor<glGetVertexAttribIiv>()(index, pname, parameters);
        }
        public static void GetVertexAttribI(uint index, uint pname, uint[] parameters)
        {
            GetDelegateFor<glGetVertexAttribIuiv>()(index, pname, parameters);
        }
        public static void VertexAttribI1(uint index, int x)
        {
            GetDelegateFor<glVertexAttribI1i>()(index, x);
        }
        public static void VertexAttribI2(uint index, int x, int y)
        {
            GetDelegateFor<glVertexAttribI2i>()(index, x, y);
        }
        public static void VertexAttribI3(uint index, int x, int y, int z)
        {
            GetDelegateFor<glVertexAttribI3i>()(index, x, y, z);
        }
        public static void VertexAttribI4(uint index, int x, int y, int z, int w)
        {
            GetDelegateFor<glVertexAttribI4i>()(index, x, y, z, w);
        }
        public static void VertexAttribI1(uint index, uint x)
        {
            GetDelegateFor<glVertexAttribI1ui>()(index, x);
        }
        public static void VertexAttribI2(uint index, uint x, uint y)
        {
            GetDelegateFor<glVertexAttribI2ui>()(index, x, y);
        }
        public static void VertexAttribI3(uint index, uint x, uint y, uint z)
        {
            GetDelegateFor<glVertexAttribI3ui>()(index, x, y, z);
        }
        public static void VertexAttribI4(uint index, uint x, uint y, uint z, uint w)
        {
            GetDelegateFor<glVertexAttribI4ui>()(index, x, y, z, w);
        }
        public static void VertexAttribI1(uint index, int[] v)
        {
            GetDelegateFor<glVertexAttribI1iv>()(index, v);
        }
        public static void VertexAttribI2(uint index, int[] v)
        {
            GetDelegateFor<glVertexAttribI2iv>()(index, v);
        }
        public static void VertexAttribI3(uint index, int[] v)
        {
            GetDelegateFor<glVertexAttribI3iv>()(index, v);
        }
        public static void VertexAttribI4(uint index, int[] v)
        {
            GetDelegateFor<glVertexAttribI4iv>()(index, v);
        }
        public static void VertexAttribI1(uint index, uint[] v)
        {
            GetDelegateFor<glVertexAttribI1uiv>()(index, v);
        }
        public static void VertexAttribI2(uint index, uint[] v)
        {
            GetDelegateFor<glVertexAttribI2uiv>()(index, v);
        }
        public static void VertexAttribI3(uint index, uint[] v)
        {
            GetDelegateFor<glVertexAttribI3uiv>()(index, v);
        }
        public static void VertexAttribI4(uint index, uint[] v)
        {
            GetDelegateFor<glVertexAttribI4uiv>()(index, v);
        }
        public static void VertexAttribI4(uint index, sbyte[] v)
        {
            GetDelegateFor<glVertexAttribI4bv>()(index, v);
        }
        public static void VertexAttribI4(uint index, short[] v)
        {
            GetDelegateFor<glVertexAttribI4sv>()(index, v);
        }
        public static void VertexAttribI4(uint index, byte[] v)
        {
            GetDelegateFor<glVertexAttribI4ubv>()(index, v);
        }
        public static void VertexAttribI4(uint index, ushort[] v)
        {
            GetDelegateFor<glVertexAttribI4usv>()(index, v);
        }
        public static void GetUniform(uint program, int location, uint[] parameters)
        {
            GetDelegateFor<glGetUniformuiv>()(program, location, parameters);
        }
        public static void BindFragDataLocation(uint program, uint color, string name)
        {
            GetDelegateFor<glBindFragDataLocation>()(program, color, name);
        }
        public static int GetFragDataLocation(uint program, string name)
        {
            return (int)GetDelegateFor<glGetFragDataLocation>()(program, name);
        }
        public static void Uniform1(int location, uint v0)
        {
            GetDelegateFor<glUniform1ui>()(location, v0);
        }
        public static void Uniform2(int location, uint v0, uint v1)
        {
            GetDelegateFor<glUniform2ui>()(location, v0, v1);
        }
        public static void Uniform3(int location, uint v0, uint v1, uint v2)
        {
            GetDelegateFor<glUniform3ui>()(location, v0, v1, v2);
        }
        public static void Uniform4(int location, uint v0, uint v1, uint v2, uint v3)
        {
            GetDelegateFor<glUniform4ui>()(location, v0, v1, v2, v3);
        }
        public static void Uniform1(int location, int count, uint[] value)
        {
            GetDelegateFor<glUniform1uiv>()(location, count, value);
        }
        public static void Uniform2(int location, int count, uint[] value)
        {
            GetDelegateFor<glUniform2uiv>()(location, count, value);
        }
        public static void Uniform3(int location, int count, uint[] value)
        {
            GetDelegateFor<glUniform3uiv>()(location, count, value);
        }
        public static void Uniform4(int location, int count, uint[] value)
        {
            GetDelegateFor<glUniform4uiv>()(location, count, value);
        }
        public static void TexParameterI(uint target, uint pname, int[] parameters)
        {
            GetDelegateFor<glTexParameterIiv>()(target, pname, parameters);
        }
        public static void TexParameterI(uint target, uint pname, uint[] parameters)
        {
            GetDelegateFor<glTexParameterIuiv>()(target, pname, parameters);
        }
        public static void GetTexParameterI(uint target, uint pname, int[] parameters)
        {
            GetDelegateFor<glGetTexParameterIiv>()(target, pname, parameters);
        }
        public static void GetTexParameterI(uint target, uint pname, uint[] parameters)
        {
            GetDelegateFor<glGetTexParameterIuiv>()(target, pname, parameters);
        }
        public static void ClearBuffer(uint buffer, int drawbuffer, int[] value)
        {
            GetDelegateFor<glClearBufferiv>()(buffer, drawbuffer, value);
        }
        public static void ClearBuffer(uint buffer, int drawbuffer, uint[] value)
        {
            GetDelegateFor<glClearBufferuiv>()(buffer, drawbuffer, value);
        }
        public static void ClearBuffer(uint buffer, int drawbuffer, float[] value)
        {
            GetDelegateFor<glClearBufferfv>()(buffer, drawbuffer, value);
        }
        public static void ClearBuffer(uint buffer, int drawbuffer, float depth, int stencil)
        {
            GetDelegateFor<glClearBufferfi>()(buffer, drawbuffer, depth, stencil);
        }
        public static string GetString(uint name, uint index)
        {
            return (string)GetDelegateFor<glGetStringi>()(name, index);
        }

        //  Delegates
        private delegate void glColorMaski(uint index, bool r, bool g, bool b, bool a);
        private delegate void glGetBooleani_v(uint target, uint index, bool[] data);
        private delegate void glGetIntegeri_v(uint target, uint index, int[] data);
        private delegate void glEnablei(uint target, uint index);
        private delegate void glDisablei(uint target, uint index);
        private delegate bool glIsEnabledi(uint target, uint index);
        private delegate void glBeginTransformFeedback(uint primitiveMode);
        private delegate void glEndTransformFeedback();
        private delegate void glBindBufferRange(uint target, uint index, uint buffer, int offset, int size);
        private delegate void glBindBufferBase(uint target, uint index, uint buffer);
        private delegate void glTransformFeedbackVaryings(uint program, int count, string[] varyings, uint bufferMode);
        private delegate void glGetTransformFeedbackVarying(uint program, uint index, int bufSize, int[] length, int[] size, uint[] type, string name);
        private delegate void glClampColor(uint target, uint clamp);
        private delegate void glBeginConditionalRender(uint id, uint mode);
        private delegate void glEndConditionalRender();
        private delegate void glVertexAttribIPointer(uint index, int size, uint type, int stride, IntPtr pointer);
        private delegate void glGetVertexAttribIiv(uint index, uint pname, int[] parameters);
        private delegate void glGetVertexAttribIuiv(uint index, uint pname, uint[] parameters);
        private delegate void glVertexAttribI1i(uint index, int x);
        private delegate void glVertexAttribI2i(uint index, int x, int y);
        private delegate void glVertexAttribI3i(uint index, int x, int y, int z);
        private delegate void glVertexAttribI4i(uint index, int x, int y, int z, int w);
        private delegate void glVertexAttribI1ui(uint index, uint x);
        private delegate void glVertexAttribI2ui(uint index, uint x, uint y);
        private delegate void glVertexAttribI3ui(uint index, uint x, uint y, uint z);
        private delegate void glVertexAttribI4ui(uint index, uint x, uint y, uint z, uint w);
        private delegate void glVertexAttribI1iv(uint index, int[] v);
        private delegate void glVertexAttribI2iv(uint index, int[] v);
        private delegate void glVertexAttribI3iv(uint index, int[] v);
        private delegate void glVertexAttribI4iv(uint index, int[] v);
        private delegate void glVertexAttribI1uiv(uint index, uint[] v);
        private delegate void glVertexAttribI2uiv(uint index, uint[] v);
        private delegate void glVertexAttribI3uiv(uint index, uint[] v);
        private delegate void glVertexAttribI4uiv(uint index, uint[] v);
        private delegate void glVertexAttribI4bv(uint index, sbyte[] v);
        private delegate void glVertexAttribI4sv(uint index, short[] v);
        private delegate void glVertexAttribI4ubv(uint index, byte[] v);
        private delegate void glVertexAttribI4usv(uint index, ushort[] v);
        private delegate void glGetUniformuiv(uint program, int location, uint[] parameters);
        private delegate void glBindFragDataLocation(uint program, uint color, string name);
        private delegate int glGetFragDataLocation(uint program, string name);
        private delegate void glUniform1ui(int location, uint v0);
        private delegate void glUniform2ui(int location, uint v0, uint v1);
        private delegate void glUniform3ui(int location, uint v0, uint v1, uint v2);
        private delegate void glUniform4ui(int location, uint v0, uint v1, uint v2, uint v3);
        private delegate void glUniform1uiv(int location, int count, uint[] value);
        private delegate void glUniform2uiv(int location, int count, uint[] value);
        private delegate void glUniform3uiv(int location, int count, uint[] value);
        private delegate void glUniform4uiv(int location, int count, uint[] value);
        private delegate void glTexParameterIiv(uint target, uint pname, int[] parameters);
        private delegate void glTexParameterIuiv(uint target, uint pname, uint[] parameters);
        private delegate void glGetTexParameterIiv(uint target, uint pname, int[] parameters);
        private delegate void glGetTexParameterIuiv(uint target, uint pname, uint[] parameters);
        private delegate void glClearBufferiv(uint buffer, int drawbuffer, int[] value);
        private delegate void glClearBufferuiv(uint buffer, int drawbuffer, uint[] value);
        private delegate void glClearBufferfv(uint buffer, int drawbuffer, float[] value);
        private delegate void glClearBufferfi(uint buffer, int drawbuffer, float depth, int stencil);
        private delegate string glGetStringi(uint name, uint index);

        //  Constants
        public const uint GL_COMPARE_REF_TO_TEXTURE = 0x884E;
        public const uint GL_CLIP_DISTANCE0 = 0x3000;
        public const uint GL_CLIP_DISTANCE1 = 0x3001;
        public const uint GL_CLIP_DISTANCE2 = 0x3002;
        public const uint GL_CLIP_DISTANCE3 = 0x3003;
        public const uint GL_CLIP_DISTANCE4 = 0x3004;
        public const uint GL_CLIP_DISTANCE5 = 0x3005;
        public const uint GL_CLIP_DISTANCE6 = 0x3006;
        public const uint GL_CLIP_DISTANCE7 = 0x3007;
        public const uint GL_MAX_CLIP_DISTANCES = 0x0D32;
        public const uint GL_MAJOR_VERSION = 0x821B;
        public const uint GL_MINOR_VERSION = 0x821C;
        public const uint GL_NUM_EXTENSIONS = 0x821D;
        public const uint GL_CONTEXT_FLAGS = 0x821E;
        public const uint GL_DEPTH_BUFFER = 0x8223;
        public const uint GL_STENCIL_BUFFER = 0x8224;
        public const uint GL_COMPRESSED_RED = 0x8225;
        public const uint GL_COMPRESSED_RG = 0x8226;
        public const uint GL_CONTEXT_FLAG_FORWARD_COMPATIBLE_BIT = 0x0001;
        public const uint GL_RGBA32F = 0x8814;
        public const uint GL_RGB32F = 0x8815;
        public const uint GL_RGBA16F = 0x881A;
        public const uint GL_RGB16F = 0x881B;
        public const uint GL_VERTEX_ATTRIB_ARRAY_INTEGER = 0x88FD;
        public const uint GL_MAX_ARRAY_TEXTURE_LAYERS = 0x88FF;
        public const uint GL_MIN_PROGRAM_TEXEL_OFFSET = 0x8904;
        public const uint GL_MAX_PROGRAM_TEXEL_OFFSET = 0x8905;
        public const uint GL_CLAMP_READ_COLOR = 0x891C;
        public const uint GL_FIXED_ONLY = 0x891D;
        public const uint GL_MAX_VARYING_COMPONENTS = 0x8B4B;
        public const uint GL_TEXTURE_1D_ARRAY = 0x8C18;
        public const uint GL_PROXY_TEXTURE_1D_ARRAY = 0x8C19;
        public const uint GL_TEXTURE_2D_ARRAY = 0x8C1A;
        public const uint GL_PROXY_TEXTURE_2D_ARRAY = 0x8C1B;
        public const uint GL_TEXTURE_BINDING_1D_ARRAY = 0x8C1C;
        public const uint GL_TEXTURE_BINDING_2D_ARRAY = 0x8C1D;
        public const uint GL_R11F_G11F_B10F = 0x8C3A;
        public const uint GL_UNSIGNED_INT_10F_11F_11F_REV = 0x8C3B;
        public const uint GL_RGB9_E5 = 0x8C3D;
        public const uint GL_UNSIGNED_INT_5_9_9_9_REV = 0x8C3E;
        public const uint GL_TEXTURE_SHARED_SIZE = 0x8C3F;
        public const uint GL_TRANSFORM_FEEDBACK_VARYING_MAX_LENGTH = 0x8C76;
        public const uint GL_TRANSFORM_FEEDBACK_BUFFER_MODE = 0x8C7F;
        public const uint GL_MAX_TRANSFORM_FEEDBACK_SEPARATE_COMPONENTS = 0x8C80;
        public const uint GL_TRANSFORM_FEEDBACK_VARYINGS = 0x8C83;
        public const uint GL_TRANSFORM_FEEDBACK_BUFFER_START = 0x8C84;
        public const uint GL_TRANSFORM_FEEDBACK_BUFFER_SIZE = 0x8C85;
        public const uint GL_PRIMITIVES_GENERATED = 0x8C87;
        public const uint GL_TRANSFORM_FEEDBACK_PRIMITIVES_WRITTEN = 0x8C88;
        public const uint GL_RASTERIZER_DISCARD = 0x8C89;
        public const uint GL_MAX_TRANSFORM_FEEDBACK_INTERLEAVED_COMPONENTS = 0x8C8A;
        public const uint GL_MAX_TRANSFORM_FEEDBACK_SEPARATE_ATTRIBS = 0x8C8B;
        public const uint GL_INTERLEAVED_ATTRIBS = 0x8C8C;
        public const uint GL_SEPARATE_ATTRIBS = 0x8C8D;
        public const uint GL_TRANSFORM_FEEDBACK_BUFFER = 0x8C8E;
        public const uint GL_TRANSFORM_FEEDBACK_BUFFER_BINDING = 0x8C8F;
        public const uint GL_RGBA32UI = 0x8D70;
        public const uint GL_RGB32UI = 0x8D71;
        public const uint GL_RGBA16UI = 0x8D76;
        public const uint GL_RGB16UI = 0x8D77;
        public const uint GL_RGBA8UI = 0x8D7C;
        public const uint GL_RGB8UI = 0x8D7D;
        public const uint GL_RGBA32I = 0x8D82;
        public const uint GL_RGB32I = 0x8D83;
        public const uint GL_RGBA16I = 0x8D88;
        public const uint GL_RGB16I = 0x8D89;
        public const uint GL_RGBA8I = 0x8D8E;
        public const uint GL_RGB8I = 0x8D8F;
        public const uint GL_RED_INTEGER = 0x8D94;
        public const uint GL_GREEN_INTEGER = 0x8D95;
        public const uint GL_BLUE_INTEGER = 0x8D96;
        public const uint GL_RGB_INTEGER = 0x8D98;
        public const uint GL_RGBA_INTEGER = 0x8D99;
        public const uint GL_BGR_INTEGER = 0x8D9A;
        public const uint GL_BGRA_INTEGER = 0x8D9B;
        public const uint GL_SAMPLER_1D_ARRAY = 0x8DC0;
        public const uint GL_SAMPLER_2D_ARRAY = 0x8DC1;
        public const uint GL_SAMPLER_1D_ARRAY_SHADOW = 0x8DC3;
        public const uint GL_SAMPLER_2D_ARRAY_SHADOW = 0x8DC4;
        public const uint GL_SAMPLER_CUBE_SHADOW = 0x8DC5;
        public const uint GL_UNSIGNED_INT_VEC2 = 0x8DC6;
        public const uint GL_UNSIGNED_INT_VEC3 = 0x8DC7;
        public const uint GL_UNSIGNED_INT_VEC4 = 0x8DC8;
        public const uint GL_INT_SAMPLER_1D = 0x8DC9;
        public const uint GL_INT_SAMPLER_2D = 0x8DCA;
        public const uint GL_INT_SAMPLER_3D = 0x8DCB;
        public const uint GL_INT_SAMPLER_CUBE = 0x8DCC;
        public const uint GL_INT_SAMPLER_1D_ARRAY = 0x8DCE;
        public const uint GL_INT_SAMPLER_2D_ARRAY = 0x8DCF;
        public const uint GL_UNSIGNED_INT_SAMPLER_1D = 0x8DD1;
        public const uint GL_UNSIGNED_INT_SAMPLER_2D = 0x8DD2;
        public const uint GL_UNSIGNED_INT_SAMPLER_3D = 0x8DD3;
        public const uint GL_UNSIGNED_INT_SAMPLER_CUBE = 0x8DD4;
        public const uint GL_UNSIGNED_INT_SAMPLER_1D_ARRAY = 0x8DD6;
        public const uint GL_UNSIGNED_INT_SAMPLER_2D_ARRAY = 0x8DD7;
        public const uint GL_QUERY_WAIT = 0x8E13;
        public const uint GL_QUERY_NO_WAIT = 0x8E14;
        public const uint GL_QUERY_BY_REGION_WAIT = 0x8E15;
        public const uint GL_QUERY_BY_REGION_NO_WAIT = 0x8E16;
        public const uint GL_BUFFER_ACCESS_FLAGS = 0x911F;
        public const uint GL_BUFFER_MAP_LENGTH = 0x9120;
        public const uint GL_BUFFER_MAP_OFFSET = 0x9121;
        public const uint GL_R8 = 0x8229;
        public const uint GL_R16 = 0x822A;
        public const uint GL_RG8 = 0x822B;
        public const uint GL_RG16 = 0x822C;
        public const uint GL_R16F = 0x822D;
        public const uint GL_R32F = 0x822E;
        public const uint GL_RG16F = 0x822F;
        public const uint GL_RG32F = 0x8230;
        public const uint GL_R8I = 0x8231;
        public const uint GL_R8UI = 0x8232;
        public const uint GL_R16I = 0x8233;
        public const uint GL_R16UI = 0x8234;
        public const uint GL_R32I = 0x8235;
        public const uint GL_R32UI = 0x8236;
        public const uint GL_RG8I = 0x8237;
        public const uint GL_RG8UI = 0x8238;
        public const uint GL_RG16I = 0x8239;
        public const uint GL_RG16UI = 0x823A;
        public const uint GL_RG32I = 0x823B;
        public const uint GL_RG32UI = 0x823C;
        public const uint GL_RG = 0x8227;
        public const uint GL_RG_INTEGER = 0x8228;

        #endregion

        #region OpenGL 3.1

        //  Methods
        public static void DrawArraysInstanced(uint mode, int first, int count, int primcount)
        {
            GetDelegateFor<glDrawArraysInstanced>()(mode, first, count, primcount);
        }
        public static void DrawElementsInstanced(uint mode, int count, uint type, IntPtr indices, int primcount)
        {
            GetDelegateFor<glDrawElementsInstanced>()(mode, count, type, indices, primcount);
        }
        public static void TexBuffer(uint target, uint internalformat, uint buffer)
        {
            GetDelegateFor<glTexBuffer>()(target, internalformat, buffer);
        }
        public static void PrimitiveRestartIndex(uint index)
        {
            GetDelegateFor<glPrimitiveRestartIndex>()(index);
        }

        //  Delegates
        private delegate void glDrawArraysInstanced(uint mode, int first, int count, int primcount);
        private delegate void glDrawElementsInstanced(uint mode, int count, uint type, IntPtr indices, int primcount);
        private delegate void glTexBuffer(uint target, uint internalformat, uint buffer);
        private delegate void glPrimitiveRestartIndex(uint index);

        //  Constants
        public const uint GL_SAMPLER_2D_RECT = 0x8B63;
        public const uint GL_SAMPLER_2D_RECT_SHADOW = 0x8B64;
        public const uint GL_SAMPLER_BUFFER = 0x8DC2;
        public const uint GL_INT_SAMPLER_2D_RECT = 0x8DCD;
        public const uint GL_INT_SAMPLER_BUFFER = 0x8DD0;
        public const uint GL_UNSIGNED_INT_SAMPLER_2D_RECT = 0x8DD5;
        public const uint GL_UNSIGNED_INT_SAMPLER_BUFFER = 0x8DD8;
        public const uint GL_TEXTURE_BUFFER = 0x8C2A;
        public const uint GL_MAX_TEXTURE_BUFFER_SIZE = 0x8C2B;
        public const uint GL_TEXTURE_BINDING_BUFFER = 0x8C2C;
        public const uint GL_TEXTURE_BUFFER_DATA_STORE_BINDING = 0x8C2D;
        public const uint GL_TEXTURE_BUFFER_FORMAT = 0x8C2E;
        public const uint GL_TEXTURE_RECTANGLE = 0x84F5;
        public const uint GL_TEXTURE_BINDING_RECTANGLE = 0x84F6;
        public const uint GL_PROXY_TEXTURE_RECTANGLE = 0x84F7;
        public const uint GL_MAX_RECTANGLE_TEXTURE_SIZE = 0x84F8;
        public const uint GL_RED_SNORM = 0x8F90;
        public const uint GL_RG_SNORM = 0x8F91;
        public const uint GL_RGB_SNORM = 0x8F92;
        public const uint GL_RGBA_SNORM = 0x8F93;
        public const uint GL_R8_SNORM = 0x8F94;
        public const uint GL_RG8_SNORM = 0x8F95;
        public const uint GL_RGB8_SNORM = 0x8F96;
        public const uint GL_RGBA8_SNORM = 0x8F97;
        public const uint GL_R16_SNORM = 0x8F98;
        public const uint GL_RG16_SNORM = 0x8F99;
        public const uint GL_RGB16_SNORM = 0x8F9A;
        public const uint GL_RGBA16_SNORM = 0x8F9B;
        public const uint GL_SIGNED_NORMALIZED = 0x8F9C;
        public const uint GL_PRIMITIVE_RESTART = 0x8F9D;
        public const uint GL_PRIMITIVE_RESTART_INDEX = 0x8F9E;

        #endregion

        #region OpenGL 3.2

        //  Methods
        public static void GetInteger64(uint target, uint index, Int64[] data)
        {
            GetDelegateFor<glGetInteger64i_v>()(target, index, data);
        }
        public static void GetBufferParameteri64(uint target, uint pname, Int64[] parameters)
        {
            GetDelegateFor<glGetBufferParameteri64v>()(target, pname, parameters);
        }
        public static void FramebufferTexture(uint target, uint attachment, uint texture, int level)
        {
            GetDelegateFor<glFramebufferTexture>()(target, attachment, texture, level);
        }

        //  Delegates
        private delegate void glGetInteger64i_v(uint target, uint index, Int64[] data);
        private delegate void glGetBufferParameteri64v(uint target, uint pname, Int64[] parameters);
        private delegate void glFramebufferTexture(uint target, uint attachment, uint texture, int level);

        //  Constants
        public const uint GL_CONTEXT_CORE_PROFILE_BIT = 0x00000001;
        public const uint GL_CONTEXT_COMPATIBILITY_PROFILE_BIT = 0x00000002;
        public const uint GL_LINES_ADJACENCY = 0x000A;
        public const uint GL_LINE_STRIP_ADJACENCY = 0x000B;
        public const uint GL_TRIANGLES_ADJACENCY = 0x000C;
        public const uint GL_TRIANGLE_STRIP_ADJACENCY = 0x000D;
        public const uint GL_PROGRAM_POINT_SIZE = 0x8642;
        public const uint GL_MAX_GEOMETRY_TEXTURE_IMAGE_UNITS = 0x8C29;
        public const uint GL_FRAMEBUFFER_ATTACHMENT_LAYERED = 0x8DA7;
        public const uint GL_FRAMEBUFFER_INCOMPLETE_LAYER_TARGETS = 0x8DA8;
        public const uint GL_GEOMETRY_SHADER = 0x8DD9;
        public const uint GL_GEOMETRY_SHADER_BIT = 0x04;
        public const uint GL_GEOMETRY_VERTICES_OUT = 0x8916;
        public const uint GL_GEOMETRY_INPUT_TYPE = 0x8917;
        public const uint GL_GEOMETRY_OUTPUT_TYPE = 0x8918;
        public const uint GL_MAX_GEOMETRY_UNIFORM_COMPONENTS = 0x8DDF;
        public const uint GL_MAX_GEOMETRY_OUTPUT_VERTICES = 0x8DE0;
        public const uint GL_MAX_GEOMETRY_TOTAL_OUTPUT_COMPONENTS = 0x8DE1;
        public const uint GL_MAX_VERTEX_OUTPUT_COMPONENTS = 0x9122;
        public const uint GL_MAX_GEOMETRY_INPUT_COMPONENTS = 0x9123;
        public const uint GL_MAX_GEOMETRY_OUTPUT_COMPONENTS = 0x9124;
        public const uint GL_MAX_FRAGMENT_INPUT_COMPONENTS = 0x9125;
        public const uint GL_CONTEXT_PROFILE_MASK = 0x9126;

        #endregion

        #region OpenGL 3.3

        //  Methods
        public static void VertexAttribDivisor(uint index, uint divisor)
        {
            GetDelegateFor<glVertexAttribDivisor>()(index, divisor);
        }

        //  Delegates
        private delegate void glVertexAttribDivisor(uint index, uint divisor);

        //  Constants
        public const uint GL_VERTEX_ATTRIB_ARRAY_DIVISOR = 0x88FE;

        #endregion

        #region OpenGL 4.0

        //  Methods        
        public static void MinSampleShading(float value)
        {
            GetDelegateFor<glMinSampleShading>()(value);
        }
        public static void BlendEquation(uint buf, uint mode)
        {
            GetDelegateFor<glBlendEquationi>()(buf, mode);
        }
        public static void BlendEquationSeparate(uint buf, uint modeRGB, uint modeAlpha)
        {
            GetDelegateFor<glBlendEquationSeparatei>()(buf, modeRGB, modeAlpha);
        }
        public static void BlendFunc(uint buf, uint src, uint dst)
        {
            GetDelegateFor<glBlendFunci>()(buf, src, dst);
        }
        public static void BlendFuncSeparate(uint buf, uint srcRGB, uint dstRGB, uint srcAlpha, uint dstAlpha)
        {
            GetDelegateFor<glBlendFuncSeparatei>()(buf, srcRGB, dstRGB, srcAlpha, dstAlpha);
        }

        //  Delegates        
        private delegate void glMinSampleShading(float value);
        private delegate void glBlendEquationi(uint buf, uint mode);
        private delegate void glBlendEquationSeparatei(uint buf, uint modeRGB, uint modeAlpha);
        private delegate void glBlendFunci(uint buf, uint src, uint dst);
        private delegate void glBlendFuncSeparatei(uint buf, uint srcRGB, uint dstRGB, uint srcAlpha, uint dstAlpha);

        //  Constants
        public const uint GL_SAMPLE_SHADING = 0x8C36;
        public const uint GL_MIN_SAMPLE_SHADING_VALUE = 0x8C37;
        public const uint GL_MIN_PROGRAM_TEXTURE_GATHER_OFFSET = 0x8E5E;
        public const uint GL_MAX_PROGRAM_TEXTURE_GATHER_OFFSET = 0x8E5F;
        public const uint GL_TEXTURE_CUBE_MAP_ARRAY = 0x9009;
        public const uint GL_TEXTURE_BINDING_CUBE_MAP_ARRAY = 0x900A;
        public const uint GL_PROXY_TEXTURE_CUBE_MAP_ARRAY = 0x900B;
        public const uint GL_SAMPLER_CUBE_MAP_ARRAY = 0x900C;
        public const uint GL_SAMPLER_CUBE_MAP_ARRAY_SHADOW = 0x900D;
        public const uint GL_INT_SAMPLER_CUBE_MAP_ARRAY = 0x900E;
        public const uint GL_UNSIGNED_INT_SAMPLER_CUBE_MAP_ARRAY = 0x900F;

        #endregion

        #region GL_EXT_texture3D

        /// <summary>
        /// Specify a three-dimensional texture subimage.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="level">The level.</param>
        /// <param name="internalformat">The internalformat.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="border">The border.</param>
        /// <param name="format">The format.</param>
        /// <param name="type">The type.</param>
        /// <param name="pixels">The pixels.</param>
        public static void TexImage3DEXT(uint target, int level, uint internalformat, uint width,
           uint height, uint depth, int border, uint format, uint type, IntPtr pixels)
        {
            GetDelegateFor<glTexImage3DEXT>()(target, level, internalformat, width, height, depth, border, format, type, pixels);
        }

        /// <summary>
        /// Texes the sub image3 DEXT.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="level">The level.</param>
        /// <param name="xoffset">The xoffset.</param>
        /// <param name="yoffset">The yoffset.</param>
        /// <param name="zoffset">The zoffset.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="format">The format.</param>
        /// <param name="type">The type.</param>
        /// <param name="pixels">The pixels.</param>
        public static void TexSubImage3DEXT(uint target, int level, int xoffset, int yoffset, int zoffset,
           uint width, uint height, uint depth, uint format, uint type, IntPtr pixels)
        {
            GetDelegateFor<glTexSubImage3DEXT>()(target, level, xoffset, yoffset, zoffset, width, height, depth, format, type, pixels);
        }

        private delegate void glTexImage3DEXT(uint target, int level, uint internalformat, uint width,
            uint height, uint depth, int border, uint format, uint type, IntPtr pixels);
        private delegate void glTexSubImage3DEXT(uint target, int level, int xoffset, int yoffset, int zoffset,
            uint width, uint height, uint depth, uint format, uint type, IntPtr pixels);

        #endregion

        #region GL_EXT_bgra

        public const uint GL_BGR_EXT = 0x80E0;
        public const uint GL_BGRA_EXT = 0x80E1;

        #endregion

        #region GL_EXT_packed_pixels

        public const uint GL_UNSIGNED_BYTE_3_3_2_EXT = 0x8032;
        public const uint GL_UNSIGNED_SHORT_4_4_4_4_EXT = 0x8033;
        public const uint GL_UNSIGNED_SHORT_5_5_5_1_EXT = 0x8034;
        public const uint GL_UNSIGNED_INT_8_8_8_8_EXT = 0x8035;
        public const uint GL_UNSIGNED_INT_10_10_10_2_EXT = 0x8036;

        #endregion

        #region GL_EXT_rescale_normal

        public const uint GL_RESCALE_NORMAL_EXT = 0x803A;

        #endregion

        #region GL_EXT_separate_specular_color

        public const uint GL_LIGHT_MODEL_COLOR_CONTROL_EXT = 0x81F8;
        public const uint GL_SINGLE_COLOR_EXT = 0x81F9;
        public const uint GL_SEPARATE_SPECULAR_COLOR_EXT = 0x81FA;

        #endregion

        #region GL_SGIS_texture_edge_clamp

        public const uint GL_CLAMP_TO_EDGE_SGIS = 0x812F;

        #endregion

        #region GL_SGIS_texture_lod

        public const uint GL_TEXTURE_MIN_LOD_SGIS = 0x813A;
        public const uint GL_TEXTURE_MAX_LOD_SGIS = 0x813B;
        public const uint GL_TEXTURE_BASE_LEVEL_SGIS = 0x813C;
        public const uint GL_TEXTURE_MAX_LEVEL_SGIS = 0x813D;

        #endregion

        #region GL_EXT_draw_range_elements

        /// <summary>
        /// Render primitives from array data.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="count">The count.</param>
        /// <param name="type">The type.</param>
        /// <param name="indices">The indices.</param>
        public static void DrawRangeElementsEXT(uint mode, uint start, uint end, uint count, uint type, IntPtr indices)
        {
            GetDelegateFor<glDrawRangeElementsEXT>()(mode, start, end, count, type, indices);
        }

        private delegate void glDrawRangeElementsEXT(uint mode, uint start, uint end, uint count, uint type, IntPtr indices);

        public const uint GL_MAX_ELEMENTS_VERTICES_EXT = 0x80E8;
        public const uint GL_MAX_ELEMENTS_INDICES_EXT = 0x80E9;

        #endregion

        #region GL_SGI_color_table

        //  Delegates
        public static void ColorTableSGI(uint target, uint internalformat, uint width, uint format, uint type, IntPtr table)
        {
            GetDelegateFor<glColorTableSGI>()(target, internalformat, width, format, type, table);
        }

        public static void ColorTableParameterSGI(uint target, uint pname, float[] parameters)
        {
            GetDelegateFor<glColorTableParameterfvSGI>()(target, pname, parameters);
        }

        public static void ColorTableParameterSGI(uint target, uint pname, int[] parameters)
        {
            GetDelegateFor<glColorTableParameterivSGI>()(target, pname, parameters);
        }

        public static void CopyColorTableSGI(uint target, uint internalformat, int x, int y, uint width)
        {
            GetDelegateFor<glCopyColorTableSGI>()(target, internalformat, x, y, width);
        }

        public static void GetColorTableSGI(uint target, uint format, uint type, IntPtr table)
        {
            GetDelegateFor<glGetColorTableSGI>()(target, format, type, table);
        }

        public static void GetColorTableParameterSGI(uint target, uint pname, float[] parameters)
        {
            GetDelegateFor<glGetColorTableParameterfvSGI>()(target, pname, parameters);
        }

        public static void GetColorTableParameterSGI(uint target, uint pname, int[] parameters)
        {
            GetDelegateFor<glGetColorTableParameterivSGI>()(target, pname, parameters);
        }

        //  Delegates
        private delegate void glColorTableSGI(uint target, uint internalformat, uint width, uint format, uint type, IntPtr table);
        private delegate void glColorTableParameterfvSGI(uint target, uint pname, float[] parameters);
        private delegate void glColorTableParameterivSGI(uint target, uint pname, int[] parameters);
        private delegate void glCopyColorTableSGI(uint target, uint internalformat, int x, int y, uint width);
        private delegate void glGetColorTableSGI(uint target, uint format, uint type, IntPtr table);
        private delegate void glGetColorTableParameterfvSGI(uint target, uint pname, float[] parameters);
        private delegate void glGetColorTableParameterivSGI(uint target, uint pname, int[] parameters);

        //  Constants
        public const uint GL_COLOR_TABLE_SGI = 0x80D0;
        public const uint GL_POST_CONVOLUTION_COLOR_TABLE_SGI = 0x80D1;
        public const uint GL_POST_COLOR_MATRIX_COLOR_TABLE_SGI = 0x80D2;
        public const uint GL_PROXY_COLOR_TABLE_SGI = 0x80D3;
        public const uint GL_PROXY_POST_CONVOLUTION_COLOR_TABLE_SGI = 0x80D4;
        public const uint GL_PROXY_POST_COLOR_MATRIX_COLOR_TABLE_SGI = 0x80D5;
        public const uint GL_COLOR_TABLE_SCALE_SGI = 0x80D6;
        public const uint GL_COLOR_TABLE_BIAS_SGI = 0x80D7;
        public const uint GL_COLOR_TABLE_FORMAT_SGI = 0x80D8;
        public const uint GL_COLOR_TABLE_WIDTH_SGI = 0x80D9;
        public const uint GL_COLOR_TABLE_RED_SIZE_SGI = 0x80DA;
        public const uint GL_COLOR_TABLE_GREEN_SIZE_SGI = 0x80DB;
        public const uint GL_COLOR_TABLE_BLUE_SIZE_SGI = 0x80DC;
        public const uint GL_COLOR_TABLE_ALPHA_SIZE_SGI = 0x80DD;
        public const uint GL_COLOR_TABLE_LUMINANCE_SIZE_SGI = 0x80DE;
        public const uint GL_COLOR_TABLE_INTENSITY_SIZE_SGI = 0x80DF;

        #endregion

        #region GL_EXT_convolution

        //  Methods.
        public static void ConvolutionFilter1DEXT(uint target, uint internalformat, int width, uint format, uint type, IntPtr image)
        {
            GetDelegateFor<glConvolutionFilter1DEXT>()(target, internalformat, width, format, type, image);
        }

        public static void ConvolutionFilter2DEXT(uint target, uint internalformat, int width, int height, uint format, uint type, IntPtr image)
        {
            GetDelegateFor<glConvolutionFilter2DEXT>()(target, internalformat, width, height, format, type, image);
        }

        public static void ConvolutionParameterEXT(uint target, uint pname, float parameters)
        {
            GetDelegateFor<glConvolutionParameterfEXT>()(target, pname, parameters);
        }

        public static void ConvolutionParameterEXT(uint target, uint pname, float[] parameters)
        {
            GetDelegateFor<glConvolutionParameterfvEXT>()(target, pname, parameters);
        }

        public static void ConvolutionParameterEXT(uint target, uint pname, int parameter)
        {
            GetDelegateFor<glConvolutionParameteriEXT>()(target, pname, parameter);
        }

        public static void ConvolutionParameterEXT(uint target, uint pname, int[] parameters)
        {
            GetDelegateFor<glConvolutionParameterivEXT>()(target, pname, parameters);
        }

        public static void CopyConvolutionFilter1DEXT(uint target, uint internalformat, int x, int y, int width)
        {
            GetDelegateFor<glCopyConvolutionFilter1DEXT>()(target, internalformat, x, y, width);
        }

        public static void CopyConvolutionFilter2DEXT(uint target, uint internalformat, int x, int y, int width, int height)
        {
            GetDelegateFor<glCopyConvolutionFilter2DEXT>()(target, internalformat, x, y, width, height);
        }

        public static void GetConvolutionFilterEXT(uint target, uint format, uint type, IntPtr image)
        {
            GetDelegateFor<glGetConvolutionFilterEXT>()(target, format, type, image);
        }

        public static void GetConvolutionParameterfvEXT(uint target, uint pname, float[] parameters)
        {
            GetDelegateFor<glGetConvolutionParameterfvEXT>()(target, pname, parameters);
        }

        public static void GetConvolutionParameterivEXT(uint target, uint pname, int[] parameters)
        {
            GetDelegateFor<glGetConvolutionParameterivEXT>()(target, pname, parameters);
        }

        public static void GetSeparableFilterEXT(uint target, uint format, uint type, IntPtr row, IntPtr column, IntPtr span)
        {
            GetDelegateFor<glGetSeparableFilterEXT>()(target, format, type, row, column, span);
        }

        public static void SeparableFilter2DEXT(uint target, uint internalformat, int width, int height, uint format, uint type, IntPtr row, IntPtr column)
        {
            GetDelegateFor<glSeparableFilter2DEXT>()(target, internalformat, width, height, format, type, row, column);
        }

        //  Delegates
        private delegate void glConvolutionFilter1DEXT(uint target, uint internalformat, int width, uint format, uint type, IntPtr image);
        private delegate void glConvolutionFilter2DEXT(uint target, uint internalformat, int width, int height, uint format, uint type, IntPtr image);
        private delegate void glConvolutionParameterfEXT(uint target, uint pname, float parameters);
        private delegate void glConvolutionParameterfvEXT(uint target, uint pname, float[] parameters);
        private delegate void glConvolutionParameteriEXT(uint target, uint pname, int parameter);
        private delegate void glConvolutionParameterivEXT(uint target, uint pname, int[] parameters);
        private delegate void glCopyConvolutionFilter1DEXT(uint target, uint internalformat, int x, int y, int width);
        private delegate void glCopyConvolutionFilter2DEXT(uint target, uint internalformat, int x, int y, int width, int height);
        private delegate void glGetConvolutionFilterEXT(uint target, uint format, uint type, IntPtr image);
        private delegate void glGetConvolutionParameterfvEXT(uint target, uint pname, float[] parameters);
        private delegate void glGetConvolutionParameterivEXT(uint target, uint pname, int[] parameters);
        private delegate void glGetSeparableFilterEXT(uint target, uint format, uint type, IntPtr row, IntPtr column, IntPtr span);
        private delegate void glSeparableFilter2DEXT(uint target, uint internalformat, int width, int height, uint format, uint type, IntPtr row, IntPtr column);

        //  Constants        
        public static uint GL_CONVOLUTION_1D_EXT = 0x8010;
        public static uint GL_CONVOLUTION_2D_EXT = 0x8011;
        public static uint GL_SEPARABLE_2D_EXT = 0x8012;
        public static uint GL_CONVOLUTION_BORDER_MODE_EXT = 0x8013;
        public static uint GL_CONVOLUTION_FILTER_SCALE_EXT = 0x8014;
        public static uint GL_CONVOLUTION_FILTER_BIAS_EXT = 0x8015;
        public static uint GL_REDUCE_EXT = 0x8016;
        public static uint GL_CONVOLUTION_FORMAT_EXT = 0x8017;
        public static uint GL_CONVOLUTION_WIDTH_EXT = 0x8018;
        public static uint GL_CONVOLUTION_HEIGHT_EXT = 0x8019;
        public static uint GL_MAX_CONVOLUTION_WIDTH_EXT = 0x801A;
        public static uint GL_MAX_CONVOLUTION_HEIGHT_EXT = 0x801B;
        public static uint GL_POST_CONVOLUTION_RED_SCALE_EXT = 0x801C;
        public static uint GL_POST_CONVOLUTION_GREEN_SCALE_EXT = 0x801D;
        public static uint GL_POST_CONVOLUTION_BLUE_SCALE_EXT = 0x801E;
        public static uint GL_POST_CONVOLUTION_ALPHA_SCALE_EXT = 0x801F;
        public static uint GL_POST_CONVOLUTION_RED_BIAS_EXT = 0x8020;
        public static uint GL_POST_CONVOLUTION_GREEN_BIAS_EXT = 0x8021;
        public static uint GL_POST_CONVOLUTION_BLUE_BIAS_EXT = 0x8022;
        public static uint GL_POST_CONVOLUTION_ALPHA_BIAS_EXT = 0x8023;

        #endregion

        #region GL_SGI_color_matrix

        public const uint GL_COLOR_MATRIX_SGI = 0x80B1;
        public const uint GL_COLOR_MATRIX_STACK_DEPTH_SGI = 0x80B2;
        public const uint GL_MAX_COLOR_MATRIX_STACK_DEPTH_SGI = 0x80B3;
        public const uint GL_POST_COLOR_MATRIX_RED_SCALE_SGI = 0x80B4;
        public const uint GL_POST_COLOR_MATRIX_GREEN_SCALE_SGI = 0x80B5;
        public const uint GL_POST_COLOR_MATRIX_BLUE_SCALE_SGI = 0x80B6;
        public const uint GL_POST_COLOR_MATRIX_ALPHA_SCALE_SGI = 0x80B7;
        public const uint GL_POST_COLOR_MATRIX_RED_BIAS_SGI = 0x80B8;
        public const uint GL_POST_COLOR_MATRIX_GREEN_BIAS_SGI = 0x80B9;
        public const uint GL_POST_COLOR_MATRIX_BLUE_BIAS_SGI = 0x80BA;
        public const uint GL_POST_COLOR_MATRIX_ALPHA_BIAS_SGI = 0x80BB;

        #endregion

        #region GL_EXT_histogram

        //  Methods
        public static void GetHistogramEXT(uint target, bool reset, uint format, uint type, IntPtr values)
        {
            GetDelegateFor<glGetHistogramEXT>()(target, reset, format, type, values);
        }

        public static void GetHistogramParameterEXT(uint target, uint pname, float[] parameters)
        {
            GetDelegateFor<glGetHistogramParameterfvEXT>()(target, pname, parameters);
        }

        public static void GetHistogramParameterEXT(uint target, uint pname, int[] parameters)
        {
            GetDelegateFor<glGetHistogramParameterivEXT>()(target, pname, parameters);
        }

        public static void GetMinmaxEXT(uint target, bool reset, uint format, uint type, IntPtr values)
        {
            GetDelegateFor<glGetMinmaxEXT>()(target, reset, format, type, values);
        }

        public static void GetMinmaxParameterfvEXT(uint target, uint pname, float[] parameters)
        {
            GetDelegateFor<glGetMinmaxParameterfvEXT>()(target, pname, parameters);
        }

        public static void GetMinmaxParameterivEXT(uint target, uint pname, int[] parameters)
        {
            GetDelegateFor<glGetMinmaxParameterivEXT>()(target, pname, parameters);
        }

        public static void HistogramEXT(uint target, int width, uint internalformat, bool sink)
        {
            GetDelegateFor<glHistogramEXT>()(target, width, internalformat, sink);
        }

        public static void MinmaxEXT(uint target, uint internalformat, bool sink)
        {
            GetDelegateFor<glMinmaxEXT>()(target, internalformat, sink);
        }

        public static void ResetHistogramEXT(uint target)
        {
            GetDelegateFor<glResetHistogramEXT>()(target);
        }

        public static void ResetMinmaxEXT(uint target)
        {
            GetDelegateFor<glResetMinmaxEXT>()(target);
        }

        //  Delegates
        private delegate void glGetHistogramEXT(uint target, bool reset, uint format, uint type, IntPtr values);
        private delegate void glGetHistogramParameterfvEXT(uint target, uint pname, float[] parameters);
        private delegate void glGetHistogramParameterivEXT(uint target, uint pname, int[] parameters);
        private delegate void glGetMinmaxEXT(uint target, bool reset, uint format, uint type, IntPtr values);
        private delegate void glGetMinmaxParameterfvEXT(uint target, uint pname, float[] parameters);
        private delegate void glGetMinmaxParameterivEXT(uint target, uint pname, int[] parameters);
        private delegate void glHistogramEXT(uint target, int width, uint internalformat, bool sink);
        private delegate void glMinmaxEXT(uint target, uint internalformat, bool sink);
        private delegate void glResetHistogramEXT(uint target);
        private delegate void glResetMinmaxEXT(uint target);

        //  Constants
        public const uint GL_HISTOGRAM_EXT = 0x8024;
        public const uint GL_PROXY_HISTOGRAM_EXT = 0x8025;
        public const uint GL_HISTOGRAM_WIDTH_EXT = 0x8026;
        public const uint GL_HISTOGRAM_FORMAT_EXT = 0x8027;
        public const uint GL_HISTOGRAM_RED_SIZE_EXT = 0x8028;
        public const uint GL_HISTOGRAM_GREEN_SIZE_EXT = 0x8029;
        public const uint GL_HISTOGRAM_BLUE_SIZE_EXT = 0x802A;
        public const uint GL_HISTOGRAM_ALPHA_SIZE_EXT = 0x802B;
        public const uint GL_HISTOGRAM_LUMINANCE_SIZE_EXT = 0x802C;
        public const uint GL_HISTOGRAM_SINK_EXT = 0x802D;
        public const uint GL_MINMAX_EXT = 0x802E;
        public const uint GL_MINMAX_FORMAT_EXT = 0x802F;
        public const uint GL_MINMAX_SINK_EXT = 0x8030;
        public const uint GL_TABLE_TOO_LARGE_EXT = 0x8031;

        #endregion

        #region GL_EXT_blend_color

        //  Methods
        public static void BlendColorEXT(float red, float green, float blue, float alpha)
        {
            GetDelegateFor<glBlendColorEXT>()(red, green, blue, alpha);
        }

        //  Delegates
        private delegate void glBlendColorEXT(float red, float green, float blue, float alpha);

        //  Constants        
        public const uint GL_CONSTANT_COLOR_EXT = 0x8001;
        public const uint GL_ONE_MINUS_CONSTANT_COLOR_EXT = 0x8002;
        public const uint GL_CONSTANT_ALPHA_EXT = 0x8003;
        public const uint GL_ONE_MINUS_CONSTANT_ALPHA_EXT = 0x8004;
        public const uint GL_BLEND_COLOR_EXT = 0x8005;

        #endregion

        #region GL_EXT_blend_minmax

        //  Methods
        public static void BlendEquationEXT(uint mode)
        {
            GetDelegateFor<glBlendEquationEXT>()(mode);
        }

        //  Delegates
        private delegate void glBlendEquationEXT(uint mode);

        //  Constants        
        public const uint GL_FUNC_ADD_EXT = 0x8006;
        public const uint GL_MIN_EXT = 0x8007;
        public const uint GL_MAX_EXT = 0x8008;
        public const uint GL_FUNC_SUBTRACT_EXT = 0x800A;
        public const uint GL_FUNC_REVERSE_SUBTRACT_EXT = 0x800B;
        public const uint GL_BLEND_EQUATION_EXT = 0x8009;

        #endregion

        #region GL_ARB_multitexture

        //  Methods
        [Obsolete("Deprecated from OpenGL version 3.0")]
        public static void ActiveTextureARB(uint texture)
        {
            GetDelegateFor<glActiveTextureARB>()(texture);
        }
        [Obsolete("Deprecated from OpenGL version 3.0")]
        public static void ClientActiveTextureARB(uint texture)
        {
            GetDelegateFor<glClientActiveTextureARB>()(texture);
        }
        [Obsolete("Deprecated from OpenGL version 3.0")]
        public static void MultiTexCoord1ARB(uint target, double s)
        {
            GetDelegateFor<glMultiTexCoord1dARB>()(target, s);
        }
        [Obsolete("Deprecated from OpenGL version 3.0")]
        public static void MultiTexCoord1ARB(uint target, double[] v)
        {
            GetDelegateFor<glMultiTexCoord1dvARB>()(target, v);
        }
        [Obsolete("Deprecated from OpenGL version 3.0")]
        public static void MultiTexCoord1ARB(uint target, float s)
        {
            GetDelegateFor<glMultiTexCoord1fARB>()(target, s);
        }
        [Obsolete("Deprecated from OpenGL version 3.0")]
        public static void MultiTexCoord1ARB(uint target, float[] v)
        {
            GetDelegateFor<glMultiTexCoord1fvARB>()(target, v);
        }
        [Obsolete("Deprecated from OpenGL version 3.0")]
        public static void MultiTexCoord1ARB(uint target, int s)
        {
            GetDelegateFor<glMultiTexCoord1iARB>()(target, s);
        }
        [Obsolete("Deprecated from OpenGL version 3.0")]
        public static void MultiTexCoord1ARB(uint target, int[] v)
        {
            GetDelegateFor<glMultiTexCoord1ivARB>()(target, v);
        }
        [Obsolete("Deprecated from OpenGL version 3.0")]
        public static void MultiTexCoord1ARB(uint target, short s)
        {
            GetDelegateFor<glMultiTexCoord1sARB>()(target, s);
        }
        [Obsolete("Deprecated from OpenGL version 3.0")]
        public static void MultiTexCoord1ARB(uint target, short[] v)
        {
            GetDelegateFor<glMultiTexCoord1svARB>()(target, v);
        }
        [Obsolete("Deprecated from OpenGL version 3.0")]
        public static void MultiTexCoord2ARB(uint target, double s, double t)
        {
            GetDelegateFor<glMultiTexCoord2dARB>()(target, s, t);
        }
        [Obsolete("Deprecated from OpenGL version 3.0")]
        public static void MultiTexCoord2ARB(uint target, double[] v)
        {
            GetDelegateFor<glMultiTexCoord2dvARB>()(target, v);
        }
        [Obsolete("Deprecated from OpenGL version 3.0")]
        public static void MultiTexCoord2ARB(uint target, float s, float t)
        {
            GetDelegateFor<glMultiTexCoord2fARB>()(target, s, t);
        }
        [Obsolete("Deprecated from OpenGL version 3.0")]
        public static void MultiTexCoord2ARB(uint target, float[] v)
        {
            GetDelegateFor<glMultiTexCoord2fvARB>()(target, v);
        }
        [Obsolete("Deprecated from OpenGL version 3.0")]
        public static void MultiTexCoord2ARB(uint target, int s, int t)
        {
            GetDelegateFor<glMultiTexCoord2iARB>()(target, s, t);
        }
        [Obsolete("Deprecated from OpenGL version 3.0")]
        public static void MultiTexCoord2ARB(uint target, int[] v)
        {
            GetDelegateFor<glMultiTexCoord2ivARB>()(target, v);
        }
        [Obsolete("Deprecated from OpenGL version 3.0")]
        public static void MultiTexCoord2ARB(uint target, short s, short t)
        {
            GetDelegateFor<glMultiTexCoord2sARB>()(target, s, t);
        }
        [Obsolete("Deprecated from OpenGL version 3.0")]
        public static void MultiTexCoord2ARB(uint target, short[] v)
        {
            GetDelegateFor<glMultiTexCoord2svARB>()(target, v);
        }
        [Obsolete("Deprecated from OpenGL version 3.0")]
        public static void MultiTexCoord3ARB(uint target, double s, double t, double r)
        {
            GetDelegateFor<glMultiTexCoord3dARB>()(target, s, t, r);
        }
        [Obsolete("Deprecated from OpenGL version 3.0")]
        public static void MultiTexCoord3ARB(uint target, double[] v)
        {
            GetDelegateFor<glMultiTexCoord3dvARB>()(target, v);
        }
        [Obsolete("Deprecated from OpenGL version 3.0")]
        public static void MultiTexCoord3ARB(uint target, float s, float t, float r)
        {
            GetDelegateFor<glMultiTexCoord3fARB>()(target, s, t, r);
        }
        [Obsolete("Deprecated from OpenGL version 3.0")]
        public static void MultiTexCoord3ARB(uint target, float[] v)
        {
            GetDelegateFor<glMultiTexCoord3fvARB>()(target, v);
        }
        [Obsolete("Deprecated from OpenGL version 3.0")]
        public static void MultiTexCoord3ARB(uint target, int s, int t, int r)
        {
            GetDelegateFor<glMultiTexCoord3iARB>()(target, s, t, r);
        }
        [Obsolete("Deprecated from OpenGL version 3.0")]
        public static void MultiTexCoord3ARB(uint target, int[] v)
        {
            GetDelegateFor<glMultiTexCoord3ivARB>()(target, v);
        }
        [Obsolete("Deprecated from OpenGL version 3.0")]
        public static void MultiTexCoord3ARB(uint target, short s, short t, short r)
        {
            GetDelegateFor<glMultiTexCoord3sARB>()(target, s, t, r);
        }
        [Obsolete("Deprecated from OpenGL version 3.0")]
        public static void MultiTexCoord3ARB(uint target, short[] v)
        {
            GetDelegateFor<glMultiTexCoord3svARB>()(target, v);
        }
        [Obsolete("Deprecated from OpenGL version 3.0")]
        public static void MultiTexCoord4ARB(uint target, double s, double t, double r, double q)
        {
            GetDelegateFor<glMultiTexCoord4dARB>()(target, s, t, r, q);
        }
        [Obsolete("Deprecated from OpenGL version 3.0")]
        public static void MultiTexCoord4ARB(uint target, double[] v)
        {
            GetDelegateFor<glMultiTexCoord4dvARB>()(target, v);
        }
        [Obsolete("Deprecated from OpenGL version 3.0")]
        public static void MultiTexCoord4ARB(uint target, float s, float t, float r, float q)
        {
            GetDelegateFor<glMultiTexCoord4fARB>()(target, s, t, r, q);
        }
        [Obsolete("Deprecated from OpenGL version 3.0")]
        public static void MultiTexCoord4ARB(uint target, float[] v)
        {
            GetDelegateFor<glMultiTexCoord4fvARB>()(target, v);
        }
        [Obsolete("Deprecated from OpenGL version 3.0")]
        public static void MultiTexCoord4ARB(uint target, int s, int t, int r, int q)
        {
            GetDelegateFor<glMultiTexCoord4iARB>()(target, s, t, r, q);
        }
        public static void MultiTexCoord4ARB(uint target, int[] v)
        {
            GetDelegateFor<glMultiTexCoord4ivARB>()(target, v);
        }
        [Obsolete("Deprecated from OpenGL version 3.0")]
        public static void MultiTexCoord4ARB(uint target, short s, short t, short r, short q)
        {
            GetDelegateFor<glMultiTexCoord4sARB>()(target, s, t, r, q);
        }
        [Obsolete("Deprecated from OpenGL version 3.0")]
        public static void MultiTexCoord4ARB(uint target, short[] v)
        {
            GetDelegateFor<glMultiTexCoord4svARB>()(target, v);
        }

        //  Delegates 
        private delegate void glActiveTextureARB(uint texture);
        private delegate void glClientActiveTextureARB(uint texture);
        private delegate void glMultiTexCoord1dARB(uint target, double s);
        private delegate void glMultiTexCoord1dvARB(uint target, double[] v);
        private delegate void glMultiTexCoord1fARB(uint target, float s);
        private delegate void glMultiTexCoord1fvARB(uint target, float[] v);
        private delegate void glMultiTexCoord1iARB(uint target, int s);
        private delegate void glMultiTexCoord1ivARB(uint target, int[] v);
        private delegate void glMultiTexCoord1sARB(uint target, short s);
        private delegate void glMultiTexCoord1svARB(uint target, short[] v);
        private delegate void glMultiTexCoord2dARB(uint target, double s, double t);
        private delegate void glMultiTexCoord2dvARB(uint target, double[] v);
        private delegate void glMultiTexCoord2fARB(uint target, float s, float t);
        private delegate void glMultiTexCoord2fvARB(uint target, float[] v);
        private delegate void glMultiTexCoord2iARB(uint target, int s, int t);
        private delegate void glMultiTexCoord2ivARB(uint target, int[] v);
        private delegate void glMultiTexCoord2sARB(uint target, short s, short t);
        private delegate void glMultiTexCoord2svARB(uint target, short[] v);
        private delegate void glMultiTexCoord3dARB(uint target, double s, double t, double r);
        private delegate void glMultiTexCoord3dvARB(uint target, double[] v);
        private delegate void glMultiTexCoord3fARB(uint target, float s, float t, float r);
        private delegate void glMultiTexCoord3fvARB(uint target, float[] v);
        private delegate void glMultiTexCoord3iARB(uint target, int s, int t, int r);
        private delegate void glMultiTexCoord3ivARB(uint target, int[] v);
        private delegate void glMultiTexCoord3sARB(uint target, short s, short t, short r);
        private delegate void glMultiTexCoord3svARB(uint target, short[] v);
        private delegate void glMultiTexCoord4dARB(uint target, double s, double t, double r, double q);
        private delegate void glMultiTexCoord4dvARB(uint target, double[] v);
        private delegate void glMultiTexCoord4fARB(uint target, float s, float t, float r, float q);
        private delegate void glMultiTexCoord4fvARB(uint target, float[] v);
        private delegate void glMultiTexCoord4iARB(uint target, int s, int t, int r, int q);
        private delegate void glMultiTexCoord4ivARB(uint target, int[] v);
        private delegate void glMultiTexCoord4sARB(uint target, short s, short t, short r, short q);
        private delegate void glMultiTexCoord4svARB(uint target, short[] v);

        //  Constants
        public const uint GL_TEXTURE0_ARB = 0x84C0;
        public const uint GL_TEXTURE1_ARB = 0x84C1;
        public const uint GL_TEXTURE2_ARB = 0x84C2;
        public const uint GL_TEXTURE3_ARB = 0x84C3;
        public const uint GL_TEXTURE4_ARB = 0x84C4;
        public const uint GL_TEXTURE5_ARB = 0x84C5;
        public const uint GL_TEXTURE6_ARB = 0x84C6;
        public const uint GL_TEXTURE7_ARB = 0x84C7;
        public const uint GL_TEXTURE8_ARB = 0x84C8;
        public const uint GL_TEXTURE9_ARB = 0x84C9;
        public const uint GL_TEXTURE10_ARB = 0x84CA;
        public const uint GL_TEXTURE11_ARB = 0x84CB;
        public const uint GL_TEXTURE12_ARB = 0x84CC;
        public const uint GL_TEXTURE13_ARB = 0x84CD;
        public const uint GL_TEXTURE14_ARB = 0x84CE;
        public const uint GL_TEXTURE15_ARB = 0x84CF;
        public const uint GL_TEXTURE16_ARB = 0x84D0;
        public const uint GL_TEXTURE17_ARB = 0x84D1;
        public const uint GL_TEXTURE18_ARB = 0x84D2;
        public const uint GL_TEXTURE19_ARB = 0x84D3;
        public const uint GL_TEXTURE20_ARB = 0x84D4;
        public const uint GL_TEXTURE21_ARB = 0x84D5;
        public const uint GL_TEXTURE22_ARB = 0x84D6;
        public const uint GL_TEXTURE23_ARB = 0x84D7;
        public const uint GL_TEXTURE24_ARB = 0x84D8;
        public const uint GL_TEXTURE25_ARB = 0x84D9;
        public const uint GL_TEXTURE26_ARB = 0x84DA;
        public const uint GL_TEXTURE27_ARB = 0x84DB;
        public const uint GL_TEXTURE28_ARB = 0x84DC;
        public const uint GL_TEXTURE29_ARB = 0x84DD;
        public const uint GL_TEXTURE30_ARB = 0x84DE;
        public const uint GL_TEXTURE31_ARB = 0x84DF;
        public const uint GL_ACTIVE_TEXTURE_ARB = 0x84E0;
        public const uint GL_CLIENT_ACTIVE_TEXTURE_ARB = 0x84E1;
        public const uint GL_MAX_TEXTURE_UNITS_ARB = 0x84E2;

        #endregion

        #region GL_ARB_texture_compression

        //  Methods
        public static void CompressedTexImage3DARB(uint target, int level, uint internalformat, int width, int height, int depth, int border, int imageSize, IntPtr data)
        {
            GetDelegateFor<glCompressedTexImage3DARB>()(target, level, internalformat, width, height, depth, border, imageSize, data);
        }
        public static void CompressedTexImage2DARB(uint target, int level, uint internalformat, int width, int height, int border, int imageSize, IntPtr data)
        {
            GetDelegateFor<glCompressedTexImage2DARB>()(target, level, internalformat, width, height, border, imageSize, data);
        }
        public static void CompressedTexImage1DARB(uint target, int level, uint internalformat, int width, int border, int imageSize, IntPtr data)
        {
            GetDelegateFor<glCompressedTexImage1DARB>()(target, level, internalformat, width, border, imageSize, data);
        }
        public static void CompressedTexSubImage3DARB(uint target, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, uint format, int imageSize, IntPtr data)
        {
            GetDelegateFor<glCompressedTexSubImage3DARB>()(target, level, xoffset, yoffset, zoffset, width, height, depth, format, imageSize, data);
        }
        public static void CompressedTexSubImage2DARB(uint target, int level, int xoffset, int yoffset, int width, int height, uint format, int imageSize, IntPtr data)
        {
            GetDelegateFor<glCompressedTexSubImage2DARB>()(target, level, xoffset, yoffset, width, height, format, imageSize, data);
        }
        public static void CompressedTexSubImage1DARB(uint target, int level, int xoffset, int width, uint format, int imageSize, IntPtr data)
        {
            GetDelegateFor<glCompressedTexSubImage1DARB>()(target, level, xoffset, width, format, imageSize, data);
        }

        //  Delegates
        private delegate void glCompressedTexImage3DARB(uint target, int level, uint internalformat, int width, int height, int depth, int border, int imageSize, IntPtr data);
        private delegate void glCompressedTexImage2DARB(uint target, int level, uint internalformat, int width, int height, int border, int imageSize, IntPtr data);
        private delegate void glCompressedTexImage1DARB(uint target, int level, uint internalformat, int width, int border, int imageSize, IntPtr data);
        private delegate void glCompressedTexSubImage3DARB(uint target, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, uint format, int imageSize, IntPtr data);
        private delegate void glCompressedTexSubImage2DARB(uint target, int level, int xoffset, int yoffset, int width, int height, uint format, int imageSize, IntPtr data);
        private delegate void glCompressedTexSubImage1DARB(uint target, int level, int xoffset, int width, uint format, int imageSize, IntPtr data);
        private delegate void glGetCompressedTexImageARB(uint target, int level, IntPtr img);

        //  Constants
        public const uint GL_COMPRESSED_ALPHA_ARB = 0x84E9;
        public const uint GL_COMPRESSED_LUMINANCE_ARB = 0x84EA;
        public const uint GL_COMPRESSED_LUMINANCE_ALPHA_ARB = 0x84EB;
        public const uint GL_COMPRESSED_INTENSITY_ARB = 0x84EC;
        public const uint GL_COMPRESSED_RGB_ARB = 0x84ED;
        public const uint GL_COMPRESSED_RGBA_ARB = 0x84EE;
        public const uint GL_TEXTURE_COMPRESSION_HINT_ARB = 0x84EF;
        public const uint GL_TEXTURE_COMPRESSED_IMAGE_SIZE_ARB = 0x86A0;
        public const uint GL_TEXTURE_COMPRESSED_ARB = 0x86A1;
        public const uint GL_NUM_COMPRESSED_TEXTURE_FORMATS_ARB = 0x86A2;
        public const uint GL_COMPRESSED_TEXTURE_FORMATS_ARB = 0x86A3;

        #endregion

        #region GL_EXT_texture_cube_map

        //  Constants
        public const uint GL_NORMAL_MAP_EXT = 0x8511;
        public const uint GL_REFLECTION_MAP_EXT = 0x8512;
        public const uint GL_TEXTURE_CUBE_MAP_EXT = 0x8513;
        public const uint GL_TEXTURE_BINDING_CUBE_MAP_EXT = 0x8514;
        public const uint GL_TEXTURE_CUBE_MAP_POSITIVE_X_EXT = 0x8515;
        public const uint GL_TEXTURE_CUBE_MAP_NEGATIVE_X_EXT = 0x8516;
        public const uint GL_TEXTURE_CUBE_MAP_POSITIVE_Y_EXT = 0x8517;
        public const uint GL_TEXTURE_CUBE_MAP_NEGATIVE_Y_EXT = 0x8518;
        public const uint GL_TEXTURE_CUBE_MAP_POSITIVE_Z_EXT = 0x8519;
        public const uint GL_TEXTURE_CUBE_MAP_NEGATIVE_Z_EXT = 0x851A;
        public const uint GL_PROXY_TEXTURE_CUBE_MAP_EXT = 0x851B;
        public const uint GL_MAX_CUBE_MAP_TEXTURE_SIZE_EXT = 0x851C;

        #endregion

        #region GL_ARB_multisample

        //  Methods
        public static void SampleCoverageARB(float value, bool invert)
        {
            GetDelegateFor<glSampleCoverageARB>()(value, invert);
        }

        //  Delegates
        private delegate void glSampleCoverageARB(float value, bool invert);

        //  Constants
        public const uint GL_MULTISAMPLE_ARB = 0x809D;
        public const uint GL_SAMPLE_ALPHA_TO_COVERAGE_ARB = 0x809E;
        public const uint GL_SAMPLE_ALPHA_TO_ONE_ARB = 0x809F;
        public const uint GL_SAMPLE_COVERAGE_ARB = 0x80A0;
        public const uint GL_SAMPLE_BUFFERS_ARB = 0x80A8;
        public const uint GL_SAMPLES_ARB = 0x80A9;
        public const uint GL_SAMPLE_COVERAGE_VALUE_ARB = 0x80AA;
        public const uint GL_SAMPLE_COVERAGE_INVERT_ARB = 0x80AB;
        public const uint GL_MULTISAMPLE_BIT_ARB = 0x20000000;

        #endregion

        #region GL_ARB_texture_env_add

        //  Appears to not have any functionality

        #endregion

        #region GL_ARB_texture_env_combine

        //  Constants
        public const uint GL_COMBINE_ARB = 0x8570;
        public const uint GL_COMBINE_RGB_ARB = 0x8571;
        public const uint GL_COMBINE_ALPHA_ARB = 0x8572;
        public const uint GL_SOURCE0_RGB_ARB = 0x8580;
        public const uint GL_SOURCE1_RGB_ARB = 0x8581;
        public const uint GL_SOURCE2_RGB_ARB = 0x8582;
        public const uint GL_SOURCE0_ALPHA_ARB = 0x8588;
        public const uint GL_SOURCE1_ALPHA_ARB = 0x8589;
        public const uint GL_SOURCE2_ALPHA_ARB = 0x858A;
        public const uint GL_OPERAND0_RGB_ARB = 0x8590;
        public const uint GL_OPERAND1_RGB_ARB = 0x8591;
        public const uint GL_OPERAND2_RGB_ARB = 0x8592;
        public const uint GL_OPERAND0_ALPHA_ARB = 0x8598;
        public const uint GL_OPERAND1_ALPHA_ARB = 0x8599;
        public const uint GL_OPERAND2_ALPHA_ARB = 0x859A;
        public const uint GL_RGB_SCALE_ARB = 0x8573;
        public const uint GL_ADD_SIGNED_ARB = 0x8574;
        public const uint GL_INTERPOLATE_ARB = 0x8575;
        public const uint GL_SUBTRACT_ARB = 0x84E7;
        public const uint GL_CONSTANT_ARB = 0x8576;
        public const uint GL_PRIMARY_COLOR_ARB = 0x8577;
        public const uint GL_PREVIOUS_ARB = 0x8578;

        #endregion

        #region GL_ARB_texture_env_dot3

        //  Constants
        public const uint GL_DOT3_RGB_ARB = 0x86AE;
        public const uint GL_DOT3_RGBA_ARB = 0x86AF;

        #endregion

        #region GL_ARB_texture_border_clamp

        //  Constants
        public const uint GL_CLAMP_TO_BORDER_ARB = 0x812D;

        #endregion

        #region GL_ARB_transpose_matrix

        //  Methods
        public static void glLoadTransposeMatrixARB(float[] m)
        {
            GetDelegateFor<glLoadTransposeMatrixfARB>()(m);
        }
        public static void glLoadTransposeMatrixARB(double[] m)
        {
            GetDelegateFor<glLoadTransposeMatrixdARB>()(m);
        }
        public static void glMultTransposeMatrixARB(float[] m)
        {
            GetDelegateFor<glMultTransposeMatrixfARB>()(m);
        }
        public static void glMultTransposeMatrixARB(double[] m)
        {
            GetDelegateFor<glMultTransposeMatrixdARB>()(m);
        }

        //  Delegates
        private delegate void glLoadTransposeMatrixfARB(float[] m);
        private delegate void glLoadTransposeMatrixdARB(double[] m);
        private delegate void glMultTransposeMatrixfARB(float[] m);
        private delegate void glMultTransposeMatrixdARB(double[] m);

        //  Constants
        public const uint GL_TRANSPOSE_MODELVIEW_MATRIX_ARB = 0x84E3;
        public const uint GL_TRANSPOSE_PROJECTION_MATRIX_ARB = 0x84E4;
        public const uint GL_TRANSPOSE_TEXTURE_MATRIX_ARB = 0x84E5;
        public const uint GL_TRANSPOSE_COLOR_MATRIX_ARB = 0x84E6;

        #endregion

        #region GL_SGIS_generate_mipmap

        //  Constants
        public const uint GL_GENERATE_MIPMAP_SGIS = 0x8191;
        public const uint GL_GENERATE_MIPMAP_HINT_SGIS = 0x8192;

        #endregion

        #region GL_NV_blend_square

        //  Appears to be empty.

        #endregion

        #region GL_ARB_depth_texture

        //  Constants
        public const uint GL_DEPTH_COMPONENT16_ARB = 0x81A5;
        public const uint GL_DEPTH_COMPONENT24_ARB = 0x81A6;
        public const uint GL_DEPTH_COMPONENT32_ARB = 0x81A7;
        public const uint GL_TEXTURE_DEPTH_SIZE_ARB = 0x884A;
        public const uint GL_DEPTH_TEXTURE_MODE_ARB = 0x884B;

        #endregion

        #region GL_ARB_shadow

        //  Constants
        public const uint GL_TEXTURE_COMPARE_MODE_ARB = 0x884C;
        public const uint GL_TEXTURE_COMPARE_FUNC_ARB = 0x884D;
        public const uint GL_COMPARE_R_TO_TEXTURE_ARB = 0x884E;

        #endregion

        #region GL_EXT_fog_coord

        //  Methods
        public static void FogCoordEXT(float coord)
        {
            GetDelegateFor<glFogCoordfEXT>()(coord);
        }
        public static void FogCoordEXT(float[] coord)
        {
            GetDelegateFor<glFogCoordfvEXT>()(coord);
        }
        public static void FogCoordEXT(double coord)
        {
            GetDelegateFor<glFogCoorddEXT>()(coord);
        }
        public static void FogCoordEXT(double[] coord)
        {
            GetDelegateFor<glFogCoorddvEXT>()(coord);
        }
        public static void FogCoordPointerEXT(uint type, int stride, IntPtr pointer)
        {
            GetDelegateFor<glFogCoordPointerEXT>()(type, stride, pointer);
        }

        //  Delegates
        private delegate void glFogCoordfEXT(float coord);
        private delegate void glFogCoordfvEXT(float[] coord);
        private delegate void glFogCoorddEXT(double coord);
        private delegate void glFogCoorddvEXT(double[] coord);
        private delegate void glFogCoordPointerEXT(uint type, int stride, IntPtr pointer);

        //  Constants
        public const uint GL_FOG_COORDINATE_SOURCE_EXT = 0x8450;
        public const uint GL_FOG_COORDINATE_EXT = 0x8451;
        public const uint GL_FRAGMENT_DEPTH_EXT = 0x8452;
        public const uint GL_CURRENT_FOG_COORDINATE_EXT = 0x8453;
        public const uint GL_FOG_COORDINATE_ARRAY_TYPE_EXT = 0x8454;
        public const uint GL_FOG_COORDINATE_ARRAY_STRIDE_EXT = 0x8455;
        public const uint GL_FOG_COORDINATE_ARRAY_POINTER_EXT = 0x8456;
        public const uint GL_FOG_COORDINATE_ARRAY_EXT = 0x8457;

        #endregion

        #region GL_EXT_multi_draw_arrays

        //  Methods
        public static void MultiDrawArraysEXT(uint mode, int[] first, int[] count, int primcount)
        {
            GetDelegateFor<glMultiDrawArraysEXT>()(mode, first, count, primcount);
        }
        public static void MultiDrawElementsEXT(uint mode, int[] count, uint type, IntPtr indices, int primcount)
        {
            GetDelegateFor<glMultiDrawElementsEXT>()(mode, count, type, indices, primcount);
        }

        //  Delegates
        private delegate void glMultiDrawArraysEXT(uint mode, int[] first, int[] count, int primcount);
        private delegate void glMultiDrawElementsEXT(uint mode, int[] count, uint type, IntPtr indices, int primcount);

        #endregion

        #region GL_ARB_point_parameters

        //  Methods
        public static void glPointParameterARB(uint pname, float parameter)
        {
            GetDelegateFor<glPointParameterfARB>()(pname, parameter);
        }
        public static void glPointParameterARB(uint pname, float[] parameters)
        {
            GetDelegateFor<glPointParameterfvARB>()(pname, parameters);
        }

        //  Delegates
        private delegate void glPointParameterfARB(uint pname, float param);
        private delegate void glPointParameterfvARB(uint pname, float[] parameters);

        //  Constants
        public const uint GL_POINT_SIZE_MIN_ARB = 0x8126;
        public const uint GL_POINT_SIZE_MAX_ARB = 0x8127;
        public const uint GL_POINT_FADE_THRESHOLD_SIZE_ARB = 0x8128;
        public const uint GL_POINT_DISTANCE_ATTENUATION_ARB = 0x8129;

        #endregion

        #region GL_EXT_secondary_color

        //  Methods
        public static void SecondaryColor3EXT(sbyte red, sbyte green, sbyte blue)
        {
            GetDelegateFor<glSecondaryColor3bEXT>()(red, green, blue);
        }
        public static void SecondaryColor3EXT(sbyte[] v)
        {
            GetDelegateFor<glSecondaryColor3bvEXT>()(v);
        }
        public static void SecondaryColor3EXT(double red, double green, double blue)
        {
            GetDelegateFor<glSecondaryColor3dEXT>()(red, green, blue);
        }
        public static void SecondaryColor3EXT(double[] v)
        {
            GetDelegateFor<glSecondaryColor3dvEXT>()(v);
        }
        public static void SecondaryColor3EXT(float red, float green, float blue)
        {
            GetDelegateFor<glSecondaryColor3fEXT>()(red, green, blue);
        }
        public static void SecondaryColor3EXT(float[] v)
        {
            GetDelegateFor<glSecondaryColor3fvEXT>()(v);
        }
        public static void SecondaryColor3EXT(int red, int green, int blue)
        {
            GetDelegateFor<glSecondaryColor3iEXT>()(red, green, blue);
        }
        public static void SecondaryColor3EXT(int[] v)
        {
            GetDelegateFor<glSecondaryColor3ivEXT>()(v);
        }
        public static void SecondaryColor3EXT(short red, short green, short blue)
        {
            GetDelegateFor<glSecondaryColor3sEXT>()(red, green, blue);
        }
        public static void SecondaryColor3EXT(short[] v)
        {
            GetDelegateFor<glSecondaryColor3svEXT>()(v);
        }
        public static void SecondaryColor3EXT(byte red, byte green, byte blue)
        {
            GetDelegateFor<glSecondaryColor3ubEXT>()(red, green, blue);
        }
        public static void SecondaryColor3EXT(byte[] v)
        {
            GetDelegateFor<glSecondaryColor3ubvEXT>()(v);
        }
        public static void SecondaryColor3EXT(uint red, uint green, uint blue)
        {
            GetDelegateFor<glSecondaryColor3uiEXT>()(red, green, blue);
        }
        public static void SecondaryColor3EXT(uint[] v)
        {
            GetDelegateFor<glSecondaryColor3uivEXT>()(v);
        }
        public static void SecondaryColor3EXT(ushort red, ushort green, ushort blue)
        {
            GetDelegateFor<glSecondaryColor3usEXT>()(red, green, blue);
        }
        public static void SecondaryColor3EXT(ushort[] v)
        {
            GetDelegateFor<glSecondaryColor3usvEXT>()(v);
        }
        public static void SecondaryColorPointerEXT(int size, uint type, int stride, IntPtr pointer)
        {
            GetDelegateFor<glSecondaryColorPointerEXT>()(size, type, stride, pointer);
        }

        //  Delegates
        private delegate void glSecondaryColor3bEXT(sbyte red, sbyte green, sbyte blue);
        private delegate void glSecondaryColor3bvEXT(sbyte[] v);
        private delegate void glSecondaryColor3dEXT(double red, double green, double blue);
        private delegate void glSecondaryColor3dvEXT(double[] v);
        private delegate void glSecondaryColor3fEXT(float red, float green, float blue);
        private delegate void glSecondaryColor3fvEXT(float[] v);
        private delegate void glSecondaryColor3iEXT(int red, int green, int blue);
        private delegate void glSecondaryColor3ivEXT(int[] v);
        private delegate void glSecondaryColor3sEXT(short red, short green, short blue);
        private delegate void glSecondaryColor3svEXT(short[] v);
        private delegate void glSecondaryColor3ubEXT(byte red, byte green, byte blue);
        private delegate void glSecondaryColor3ubvEXT(byte[] v);
        private delegate void glSecondaryColor3uiEXT(uint red, uint green, uint blue);
        private delegate void glSecondaryColor3uivEXT(uint[] v);
        private delegate void glSecondaryColor3usEXT(ushort red, ushort green, ushort blue);
        private delegate void glSecondaryColor3usvEXT(ushort[] v);
        private delegate void glSecondaryColorPointerEXT(int size, uint type, int stride, IntPtr pointer);

        //  Constants        
        public const uint GL_COLOR_SUM_EXT = 0x8458;
        public const uint GL_CURRENT_SECONDARY_COLOR_EXT = 0x8459;
        public const uint GL_SECONDARY_COLOR_ARRAY_SIZE_EXT = 0x845A;
        public const uint GL_SECONDARY_COLOR_ARRAY_TYPE_EXT = 0x845B;
        public const uint GL_SECONDARY_COLOR_ARRAY_STRIDE_EXT = 0x845C;
        public const uint GL_SECONDARY_COLOR_ARRAY_POINTER_EXT = 0x845D;
        public const uint GL_SECONDARY_COLOR_ARRAY_EXT = 0x845E;

        #endregion

        #region  GL_EXT_blend_func_separate

        //  Methods
        public static void BlendFuncSeparateEXT(uint sfactorRGB, uint dfactorRGB, uint sfactorAlpha, uint dfactorAlpha)
        {
            GetDelegateFor<glBlendFuncSeparateEXT>()(sfactorRGB, dfactorRGB, sfactorAlpha, dfactorAlpha);
        }

        //  Delegates
        private delegate void glBlendFuncSeparateEXT(uint sfactorRGB, uint dfactorRGB, uint sfactorAlpha, uint dfactorAlpha);

        //  Constants
        public const uint GL_BLEND_DST_RGB_EXT = 0x80C8;
        public const uint GL_BLEND_SRC_RGB_EXT = 0x80C9;
        public const uint GL_BLEND_DST_ALPHA_EXT = 0x80CA;
        public const uint GL_BLEND_SRC_ALPHA_EXT = 0x80CB;

        #endregion

        #region GL_EXT_stencil_wrap

        //  Constants
        public const uint GL_INCR_WRAP_EXT = 0x8507;
        public const uint GL_DECR_WRAP_EXT = 0x8508;

        #endregion

        #region GL_ARB_texture_env_crossbar

        //  No methods or constants.

        #endregion

        #region GL_EXT_texture_lod_bias

        //  Constants
        public const uint GL_MAX_TEXTURE_LOD_BIAS_EXT = 0x84FD;
        public const uint GL_TEXTURE_FILTER_CONTROL_EXT = 0x8500;
        public const uint GL_TEXTURE_LOD_BIAS_EXT = 0x8501;

        #endregion

        #region GL_ARB_texture_mirrored_repeat

        //  Constants
        public const uint GL_MIRRORED_REPEAT_ARB = 0x8370;

        #endregion

        #region GL_ARB_window_pos

        //  Methods
        public static void WindowPos2ARB(double x, double y)
        {
            GetDelegateFor<glWindowPos2dARB>()(x, y);
        }
        public static void WindowPos2ARB(double[] v)
        {
            GetDelegateFor<glWindowPos2dvARB>()(v);
        }
        public static void WindowPos2ARB(float x, float y)
        {
            GetDelegateFor<glWindowPos2fARB>()(x, y);
        }
        public static void WindowPos2ARB(float[] v)
        {
            GetDelegateFor<glWindowPos2fvARB>()(v);
        }
        public static void WindowPos2ARB(int x, int y)
        {
            GetDelegateFor<glWindowPos2iARB>()(x, y);
        }
        public static void WindowPos2ARB(int[] v)
        {
            GetDelegateFor<glWindowPos2ivARB>()(v);
        }
        public static void WindowPos2ARB(short x, short y)
        {
            GetDelegateFor<glWindowPos2sARB>()(x, y);
        }
        public static void WindowPos2ARB(short[] v)
        {
            GetDelegateFor<glWindowPos2svARB>()(v);
        }
        public static void WindowPos3ARB(double x, double y, double z)
        {
            GetDelegateFor<glWindowPos3dARB>()(x, y, z);
        }
        public static void WindowPos3ARB(double[] v)
        {
            GetDelegateFor<glWindowPos3dvARB>()(v);
        }
        public static void WindowPos3ARB(float x, float y, float z)
        {
            GetDelegateFor<glWindowPos3fARB>()(x, y, z);
        }
        public static void WindowPos3ARB(float[] v)
        {
            GetDelegateFor<glWindowPos3fvARB>()(v);
        }
        public static void WindowPos3ARB(int x, int y, int z)
        {
            GetDelegateFor<glWindowPos3iARB>()(x, y, z);
        }
        public static void WindowPos3ARB(int[] v)
        {
            GetDelegateFor<glWindowPos3ivARB>()(v);
        }
        public static void WindowPos3ARB(short x, short y, short z)
        {
            GetDelegateFor<glWindowPos3sARB>()(x, y, z);
        }
        public static void WindowPos3ARB(short[] v)
        {
            GetDelegateFor<glWindowPos3svARB>()(v);
        }

        //  Delegates
        private delegate void glWindowPos2dARB(double x, double y);
        private delegate void glWindowPos2dvARB(double[] v);
        private delegate void glWindowPos2fARB(float x, float y);
        private delegate void glWindowPos2fvARB(float[] v);
        private delegate void glWindowPos2iARB(int x, int y);
        private delegate void glWindowPos2ivARB(int[] v);
        private delegate void glWindowPos2sARB(short x, short y);
        private delegate void glWindowPos2svARB(short[] v);
        private delegate void glWindowPos3dARB(double x, double y, double z);
        private delegate void glWindowPos3dvARB(double[] v);
        private delegate void glWindowPos3fARB(float x, float y, float z);
        private delegate void glWindowPos3fvARB(float[] v);
        private delegate void glWindowPos3iARB(int x, int y, int z);
        private delegate void glWindowPos3ivARB(int[] v);
        private delegate void glWindowPos3sARB(short x, short y, short z);
        private delegate void glWindowPos3svARB(short[] v);

        #endregion

        #region GL_ARB_vertex_buffer_object

        //  Methods
        public static void BindBufferARB(uint target, uint buffer)
        {
            GetDelegateFor<glBindBufferARB>()(target, buffer);
        }
        public static void DeleteBuffersARB(int n, uint[] buffers)
        {
            GetDelegateFor<glDeleteBuffersARB>()(n, buffers);
        }
        public static void GenBuffersARB(int n, uint[] buffers)
        {
            GetDelegateFor<glGenBuffersARB>()(n, buffers);
        }
        public static bool IsBufferARB(uint buffer)
        {
            return (bool)GetDelegateFor<glIsBufferARB>()(buffer);
        }
        public static void BufferDataARB(uint target, uint size, IntPtr data, uint usage)
        {
            GetDelegateFor<glBufferDataARB>()(target, size, data, usage);
        }
        public static void BufferSubDataARB(uint target, uint offset, uint size, IntPtr data)
        {
            GetDelegateFor<glBufferSubDataARB>()(target, offset, size, data);
        }
        public static void GetBufferSubDataARB(uint target, uint offset, uint size, IntPtr data)
        {
            GetDelegateFor<glGetBufferSubDataARB>()(target, offset, size, data);
        }
        public static IntPtr MapBufferARB(uint target, uint access)
        {
            return (IntPtr)GetDelegateFor<glMapBufferARB>()(target, access);
        }
        public static bool UnmapBufferARB(uint target)
        {
            return (bool)GetDelegateFor<glUnmapBufferARB>()(target);
        }
        public static void GetBufferParameterARB(uint target, uint pname, int[] parameters)
        {
            GetDelegateFor<glGetBufferParameterivARB>()(target, pname, parameters);
        }
        public static void GetBufferPointerARB(uint target, uint pname, IntPtr parameters)
        {
            GetDelegateFor<glGetBufferPointervARB>()(target, pname, parameters);
        }

        //  Delegates
        private delegate void glBindBufferARB(uint target, uint buffer);
        private delegate void glDeleteBuffersARB(int n, uint[] buffers);
        private delegate void glGenBuffersARB(int n, uint[] buffers);
        private delegate bool glIsBufferARB(uint buffer);
        private delegate void glBufferDataARB(uint target, uint size, IntPtr data, uint usage);
        private delegate void glBufferSubDataARB(uint target, uint offset, uint size, IntPtr data);
        private delegate void glGetBufferSubDataARB(uint target, uint offset, uint size, IntPtr data);
        private delegate IntPtr glMapBufferARB(uint target, uint access);
        private delegate bool glUnmapBufferARB(uint target);
        private delegate void glGetBufferParameterivARB(uint target, uint pname, int[] parameters);
        private delegate void glGetBufferPointervARB(uint target, uint pname, IntPtr parameters);

        //  Constants
        public const uint GL_BUFFER_SIZE_ARB = 0x8764;
        public const uint GL_BUFFER_USAGE_ARB = 0x8765;
        public const uint GL_ARRAY_BUFFER_ARB = 0x8892;
        public const uint GL_ELEMENT_ARRAY_BUFFER_ARB = 0x8893;
        public const uint GL_ARRAY_BUFFER_BINDING_ARB = 0x8894;
        public const uint GL_ELEMENT_ARRAY_BUFFER_BINDING_ARB = 0x8895;
        public const uint GL_VERTEX_ARRAY_BUFFER_BINDING_ARB = 0x8896;
        public const uint GL_NORMAL_ARRAY_BUFFER_BINDING_ARB = 0x8897;
        public const uint GL_COLOR_ARRAY_BUFFER_BINDING_ARB = 0x8898;
        public const uint GL_INDEX_ARRAY_BUFFER_BINDING_ARB = 0x8899;
        public const uint GL_TEXTURE_COORD_ARRAY_BUFFER_BINDING_ARB = 0x889A;
        public const uint GL_EDGE_FLAG_ARRAY_BUFFER_BINDING_ARB = 0x889B;
        public const uint GL_SECONDARY_COLOR_ARRAY_BUFFER_BINDING_ARB = 0x889C;
        public const uint GL_FOG_COORDINATE_ARRAY_BUFFER_BINDING_ARB = 0x889D;
        public const uint GL_WEIGHT_ARRAY_BUFFER_BINDING_ARB = 0x889E;
        public const uint GL_VERTEX_ATTRIB_ARRAY_BUFFER_BINDING_ARB = 0x889F;
        public const uint GL_READ_ONLY_ARB = 0x88B8;
        public const uint GL_WRITE_ONLY_ARB = 0x88B9;
        public const uint GL_READ_WRITE_ARB = 0x88BA;
        public const uint GL_BUFFER_ACCESS_ARB = 0x88BB;
        public const uint GL_BUFFER_MAPPED_ARB = 0x88BC;
        public const uint GL_BUFFER_MAP_POINTER_ARB = 0x88BD;
        public const uint GL_STREAM_DRAW_ARB = 0x88E0;
        public const uint GL_STREAM_READ_ARB = 0x88E1;
        public const uint GL_STREAM_COPY_ARB = 0x88E2;
        public const uint GL_STATIC_DRAW_ARB = 0x88E4;
        public const uint GL_STATIC_READ_ARB = 0x88E5;
        public const uint GL_STATIC_COPY_ARB = 0x88E6;
        public const uint GL_DYNAMIC_DRAW_ARB = 0x88E8;
        public const uint GL_DYNAMIC_READ_ARB = 0x88E9;
        public const uint GL_DYNAMIC_COPY_ARB = 0x88EA;
        #endregion

        #region GL_ARB_occlusion_query

        //  Methods
        public static void GenQueriesARB(int n, uint[] ids)
        {
            GetDelegateFor<glGenQueriesARB>()(n, ids);
        }
        public static void DeleteQueriesARB(int n, uint[] ids)
        {
            GetDelegateFor<glDeleteQueriesARB>()(n, ids);
        }
        public static bool IsQueryARB(uint id)
        {
            return (bool)GetDelegateFor<glIsQueryARB>()(id);
        }
        public static void BeginQueryARB(uint target, uint id)
        {
            GetDelegateFor<glBeginQueryARB>()(target, id);
        }
        public static void EndQueryARB(uint target)
        {
            GetDelegateFor<glEndQueryARB>()(target);
        }
        public static void GetQueryARB(uint target, uint pname, int[] parameters)
        {
            GetDelegateFor<glGetQueryivARB>()(target, pname, parameters);
        }
        public static void GetQueryObjectARB(uint id, uint pname, int[] parameters)
        {
            GetDelegateFor<glGetQueryObjectivARB>()(id, pname, parameters);
        }
        public static void GetQueryObjectARB(uint id, uint pname, uint[] parameters)
        {
            GetDelegateFor<glGetQueryObjectuivARB>()(id, pname, parameters);
        }

        //  Delegates
        private delegate void glGenQueriesARB(int n, uint[] ids);
        private delegate void glDeleteQueriesARB(int n, uint[] ids);
        private delegate bool glIsQueryARB(uint id);
        private delegate void glBeginQueryARB(uint target, uint id);
        private delegate void glEndQueryARB(uint target);
        private delegate void glGetQueryivARB(uint target, uint pname, int[] parameters);
        private delegate void glGetQueryObjectivARB(uint id, uint pname, int[] parameters);
        private delegate void glGetQueryObjectuivARB(uint id, uint pname, uint[] parameters);

        //  Constants
        public const uint GL_QUERY_COUNTER_BITS_ARB = 0x8864;
        public const uint GL_CURRENT_QUERY_ARB = 0x8865;
        public const uint GL_QUERY_RESULT_ARB = 0x8866;
        public const uint GL_QUERY_RESULT_AVAILABLE_ARB = 0x8867;
        public const uint GL_SAMPLES_PASSED_ARB = 0x8914;
        public const uint GL_ANY_SAMPLES_PASSED = 0x8C2F;

        #endregion

        #region GL_ARB_shader_objects

        //  Methods
        public static void DeleteObjectARB(uint obj)
        {
            GetDelegateFor<glDeleteObjectARB>()(obj);
        }
        public static uint GetHandleARB(uint pname)
        {
            return (uint)GetDelegateFor<glGetHandleARB>()(pname);
        }
        public static void DetachObjectARB(uint containerObj, uint attachedObj)
        {
            GetDelegateFor<glDetachObjectARB>()(containerObj, attachedObj);
        }
        public static uint CreateShaderObjectARB(uint shaderType)
        {
            return (uint)GetDelegateFor<glCreateShaderObjectARB>()(shaderType);
        }
        public static void ShaderSourceARB(uint shaderObj, int count, string[] source, ref int length)
        {
            GetDelegateFor<glShaderSourceARB>()(shaderObj, count, source, ref length);
        }
        public static void CompileShaderARB(uint shaderObj)
        {
            GetDelegateFor<glCompileShaderARB>()(shaderObj);
        }
        public static uint CreateProgramObjectARB()
        {
            return (uint)GetDelegateFor<glCreateProgramObjectARB>()();
        }
        public static void AttachObjectARB(uint containerObj, uint obj)
        {
            GetDelegateFor<glAttachObjectARB>()(containerObj, obj);
        }
        public static void LinkProgramARB(uint programObj)
        {
            GetDelegateFor<glLinkProgramARB>()(programObj);
        }
        public static void UseProgramObjectARB(uint programObj)
        {
            GetDelegateFor<glUseProgramObjectARB>()(programObj);
        }
        public static void ValidateProgramARB(uint programObj)
        {
            GetDelegateFor<glValidateProgramARB>()(programObj);
        }
        public static void Uniform1ARB(int location, float v0)
        {
            GetDelegateFor<glUniform1fARB>()(location, v0);
        }
        public static void Uniform2ARB(int location, float v0, float v1)
        {
            GetDelegateFor<glUniform2fARB>()(location, v0, v1);
        }
        public static void Uniform3ARB(int location, float v0, float v1, float v2)
        {
            GetDelegateFor<glUniform3fARB>()(location, v0, v1, v2);
        }
        public static void Uniform4ARB(int location, float v0, float v1, float v2, float v3)
        {
            GetDelegateFor<glUniform4fARB>()(location, v0, v1, v2, v3);
        }
        public static void Uniform1ARB(int location, int v0)
        {
            GetDelegateFor<glUniform1iARB>()(location, v0);
        }
        public static void Uniform2ARB(int location, int v0, int v1)
        {
            GetDelegateFor<glUniform2iARB>()(location, v0, v1);
        }
        public static void Uniform3ARB(int location, int v0, int v1, int v2)
        {
            GetDelegateFor<glUniform3iARB>()(location, v0, v1, v2);
        }
        public static void Uniform4ARB(int location, int v0, int v1, int v2, int v3)
        {
            GetDelegateFor<glUniform4iARB>()(location, v0, v1, v2, v3);
        }
        public static void Uniform1ARB(int location, int count, float[] value)
        {
            GetDelegateFor<glUniform1fvARB>()(location, count, value);
        }
        public static void Uniform2ARB(int location, int count, float[] value)
        {
            GetDelegateFor<glUniform2fvARB>()(location, count, value);
        }
        public static void Uniform3ARB(int location, int count, float[] value)
        {
            GetDelegateFor<glUniform3fvARB>()(location, count, value);
        }
        public static void Uniform4ARB(int location, int count, float[] value)
        {
            GetDelegateFor<glUniform4fvARB>()(location, count, value);
        }
        public static void Uniform1ARB(int location, int count, int[] value)
        {
            GetDelegateFor<glUniform1ivARB>()(location, count, value);
        }
        public static void Uniform2ARB(int location, int count, int[] value)
        {
            GetDelegateFor<glUniform2ivARB>()(location, count, value);
        }
        public static void Uniform3ARB(int location, int count, int[] value)
        {
            GetDelegateFor<glUniform3ivARB>()(location, count, value);
        }
        public static void Uniform4ARB(int location, int count, int[] value)
        {
            GetDelegateFor<glUniform4ivARB>()(location, count, value);
        }
        public static void UniformMatrix2ARB(int location, int count, bool transpose, float[] value)
        {
            GetDelegateFor<glUniformMatrix2fvARB>()(location, count, transpose, value);
        }
        public static void UniformMatrix3ARB(int location, int count, bool transpose, float[] value)
        {
            GetDelegateFor<glUniformMatrix3fvARB>()(location, count, transpose, value);
        }
        public static void UniformMatrix4ARB(int location, int count, bool transpose, float[] value)
        {
            GetDelegateFor<glUniformMatrix4fvARB>()(location, count, transpose, value);
        }
        public static void GetObjectParameterARB(uint obj, uint pname, float[] parameters)
        {
            GetDelegateFor<glGetObjectParameterfvARB>()(obj, pname, parameters);
        }
        public static void GetObjectParameterARB(uint obj, uint pname, int[] parameters)
        {
            GetDelegateFor<glGetObjectParameterivARB>()(obj, pname, parameters);
        }
        public static void GetInfoLogARB(uint obj, int maxLength, ref int length, string infoLog)
        {
            GetDelegateFor<glGetInfoLogARB>()(obj, maxLength, ref length, infoLog);
        }
        public static void GetAttachedObjectsARB(uint containerObj, int maxCount, ref int count, ref uint obj)
        {
            GetDelegateFor<glGetAttachedObjectsARB>()(containerObj, maxCount, ref count, ref obj);
        }
        public static int GetUniformLocationARB(uint programObj, string name)
        {
            return (int)GetDelegateFor<glGetUniformLocationARB>()(programObj, name);
        }
        public static void GetActiveUniformARB(uint programObj, uint index, int maxLength, ref int length, ref int size, ref uint type, string name)
        {
            GetDelegateFor<glGetActiveUniformARB>()(programObj, index, maxLength, ref length, ref size, ref type, name);
        }
        public static void GetUniformARB(uint programObj, int location, float[] parameters)
        {
            GetDelegateFor<glGetUniformfvARB>()(programObj, location, parameters);
        }
        public static void GetUniformARB(uint programObj, int location, int[] parameters)
        {
            GetDelegateFor<glGetUniformivARB>()(programObj, location, parameters);
        }
        public static void GetShaderSourceARB(uint obj, int maxLength, ref int length, string source)
        {
            GetDelegateFor<glGetShaderSourceARB>()(obj, maxLength, ref length, source);
        }

        //  Delegates
        private delegate void glDeleteObjectARB(uint obj);
        private delegate uint glGetHandleARB(uint pname);
        private delegate void glDetachObjectARB(uint containerObj, uint attachedObj);
        private delegate uint glCreateShaderObjectARB(uint shaderType);
        private delegate void glShaderSourceARB(uint shaderObj, int count, string[] source, ref int length);
        private delegate void glCompileShaderARB(uint shaderObj);
        private delegate uint glCreateProgramObjectARB();
        private delegate void glAttachObjectARB(uint containerObj, uint obj);
        private delegate void glLinkProgramARB(uint programObj);
        private delegate void glUseProgramObjectARB(uint programObj);
        private delegate void glValidateProgramARB(uint programObj);
        private delegate void glUniform1fARB(int location, float v0);
        private delegate void glUniform2fARB(int location, float v0, float v1);
        private delegate void glUniform3fARB(int location, float v0, float v1, float v2);
        private delegate void glUniform4fARB(int location, float v0, float v1, float v2, float v3);
        private delegate void glUniform1iARB(int location, int v0);
        private delegate void glUniform2iARB(int location, int v0, int v1);
        private delegate void glUniform3iARB(int location, int v0, int v1, int v2);
        private delegate void glUniform4iARB(int location, int v0, int v1, int v2, int v3);
        private delegate void glUniform1fvARB(int location, int count, float[] value);
        private delegate void glUniform2fvARB(int location, int count, float[] value);
        private delegate void glUniform3fvARB(int location, int count, float[] value);
        private delegate void glUniform4fvARB(int location, int count, float[] value);
        private delegate void glUniform1ivARB(int location, int count, int[] value);
        private delegate void glUniform2ivARB(int location, int count, int[] value);
        private delegate void glUniform3ivARB(int location, int count, int[] value);
        private delegate void glUniform4ivARB(int location, int count, int[] value);
        private delegate void glUniformMatrix2fvARB(int location, int count, bool transpose, float[] value);
        private delegate void glUniformMatrix3fvARB(int location, int count, bool transpose, float[] value);
        private delegate void glUniformMatrix4fvARB(int location, int count, bool transpose, float[] value);
        private delegate void glGetObjectParameterfvARB(uint obj, uint pname, float[] parameters);
        private delegate void glGetObjectParameterivARB(uint obj, uint pname, int[] parameters);
        private delegate void glGetInfoLogARB(uint obj, int maxLength, ref int length, string infoLog);
        private delegate void glGetAttachedObjectsARB(uint containerObj, int maxCount, ref int count, ref uint obj);
        private delegate int glGetUniformLocationARB(uint programObj, string name);
        private delegate void glGetActiveUniformARB(uint programObj, uint index, int maxLength, ref int length, ref int size, ref uint type, string name);
        private delegate void glGetUniformfvARB(uint programObj, int location, float[] parameters);
        private delegate void glGetUniformivARB(uint programObj, int location, int[] parameters);
        private delegate void glGetShaderSourceARB(uint obj, int maxLength, ref int length, string source);

        //  Constants
        public const uint GL_PROGRAM_OBJECT_ARB = 0x8B40;
        public const uint GL_SHADER_OBJECT_ARB = 0x8B48;
        public const uint GL_OBJECT_TYPE_ARB = 0x8B4E;
        public const uint GL_OBJECT_SUBTYPE_ARB = 0x8B4F;
        public const uint GL_FLOAT_VEC2_ARB = 0x8B50;
        public const uint GL_FLOAT_VEC3_ARB = 0x8B51;
        public const uint GL_FLOAT_VEC4_ARB = 0x8B52;
        public const uint GL_INT_VEC2_ARB = 0x8B53;
        public const uint GL_INT_VEC3_ARB = 0x8B54;
        public const uint GL_INT_VEC4_ARB = 0x8B55;
        public const uint GL_BOOL_ARB = 0x8B56;
        public const uint GL_BOOL_VEC2_ARB = 0x8B57;
        public const uint GL_BOOL_VEC3_ARB = 0x8B58;
        public const uint GL_BOOL_VEC4_ARB = 0x8B59;
        public const uint GL_FLOAT_MAT2_ARB = 0x8B5A;
        public const uint GL_FLOAT_MAT3_ARB = 0x8B5B;
        public const uint GL_FLOAT_MAT4_ARB = 0x8B5C;
        public const uint GL_SAMPLER_1D_ARB = 0x8B5D;
        public const uint GL_SAMPLER_2D_ARB = 0x8B5E;
        public const uint GL_SAMPLER_3D_ARB = 0x8B5F;
        public const uint GL_SAMPLER_CUBE_ARB = 0x8B60;
        public const uint GL_SAMPLER_1D_SHADOW_ARB = 0x8B61;
        public const uint GL_SAMPLER_2D_SHADOW_ARB = 0x8B62;
        public const uint GL_SAMPLER_2D_RECT_ARB = 0x8B63;
        public const uint GL_SAMPLER_2D_RECT_SHADOW_ARB = 0x8B64;
        public const uint GL_OBJECT_DELETE_STATUS_ARB = 0x8B80;
        public const uint GL_OBJECT_COMPILE_STATUS_ARB = 0x8B81;
        public const uint GL_OBJECT_LINK_STATUS_ARB = 0x8B82;
        public const uint GL_OBJECT_VALIDATE_STATUS_ARB = 0x8B83;
        public const uint GL_OBJECT_INFO_LOG_LENGTH_ARB = 0x8B84;
        public const uint GL_OBJECT_ATTACHED_OBJECTS_ARB = 0x8B85;
        public const uint GL_OBJECT_ACTIVE_UNIFORMS_ARB = 0x8B86;
        public const uint GL_OBJECT_ACTIVE_UNIFORM_MAX_LENGTH_ARB = 0x8B87;
        public const uint GL_OBJECT_SHADER_SOURCE_LENGTH_ARB = 0x8B88;

        #endregion

        #region GL_ARB_vertex_program

        //  Methods
        public static void VertexAttrib1ARB(uint index, double x)
        {
            GetDelegateFor<glVertexAttrib1dARB>()(index, x);
        }
        public static void VertexAttrib1ARB(uint index, double[] v)
        {
            GetDelegateFor<glVertexAttrib1dvARB>()(index, v);
        }
        public static void VertexAttrib1ARB(uint index, float x)
        {
            GetDelegateFor<glVertexAttrib1fARB>()(index, x);
        }
        public static void VertexAttrib1ARB(uint index, float[] v)
        {
            GetDelegateFor<glVertexAttrib1fvARB>()(index, v);
        }
        public static void VertexAttrib1ARB(uint index, short x)
        {
            GetDelegateFor<glVertexAttrib1sARB>()(index, x);
        }
        public static void VertexAttrib1ARB(uint index, short[] v)
        {
            GetDelegateFor<glVertexAttrib1svARB>()(index, v);
        }
        public static void VertexAttrib2ARB(uint index, double x, double y)
        {
            GetDelegateFor<glVertexAttrib2dARB>()(index, x, y);
        }
        public static void VertexAttrib2ARB(uint index, double[] v)
        {
            GetDelegateFor<glVertexAttrib2dvARB>()(index, v);
        }
        public static void VertexAttrib2ARB(uint index, float x, float y)
        {
            GetDelegateFor<glVertexAttrib2fARB>()(index, x, y);
        }
        public static void VertexAttrib2ARB(uint index, float[] v)
        {
            GetDelegateFor<glVertexAttrib2fvARB>()(index, v);
        }
        public static void VertexAttrib2ARB(uint index, short x, short y)
        {
            GetDelegateFor<glVertexAttrib2sARB>()(index, x, y);
        }
        public static void VertexAttrib2ARB(uint index, short[] v)
        {
            GetDelegateFor<glVertexAttrib2svARB>()(index, v);
        }
        public static void VertexAttrib3ARB(uint index, double x, double y, double z)
        {
            GetDelegateFor<glVertexAttrib3dARB>()(index, x, y, z);
        }
        public static void VertexAttrib3ARB(uint index, double[] v)
        {
            GetDelegateFor<glVertexAttrib3dvARB>()(index, v);
        }
        public static void VertexAttrib3ARB(uint index, float x, float y, float z)
        {
            GetDelegateFor<glVertexAttrib3fARB>()(index, x, y, z);
        }
        public static void VertexAttrib3ARB(uint index, float[] v)
        {
            GetDelegateFor<glVertexAttrib3fvARB>()(index, v);
        }
        public static void VertexAttrib3ARB(uint index, short x, short y, short z)
        {
            GetDelegateFor<glVertexAttrib3sARB>()(index, x, y, z);
        }
        public static void VertexAttrib3ARB(uint index, short[] v)
        {
            GetDelegateFor<glVertexAttrib3svARB>()(index, v);
        }
        public static void VertexAttrib4NARB(uint index, sbyte[] v)
        {
            GetDelegateFor<glVertexAttrib4NbvARB>()(index, v);
        }
        public static void VertexAttrib4NARB(uint index, int[] v)
        {
            GetDelegateFor<glVertexAttrib4NivARB>()(index, v);
        }
        public static void VertexAttrib4NARB(uint index, short[] v)
        {
            GetDelegateFor<glVertexAttrib4NsvARB>()(index, v);
        }
        public static void VertexAttrib4NARB(uint index, byte x, byte y, byte z, byte w)
        {
            GetDelegateFor<glVertexAttrib4NubARB>()(index, x, y, z, w);
        }
        public static void VertexAttrib4NARB(uint index, byte[] v)
        {
            GetDelegateFor<glVertexAttrib4NubvARB>()(index, v);
        }
        public static void VertexAttrib4NARB(uint index, uint[] v)
        {
            GetDelegateFor<glVertexAttrib4NuivARB>()(index, v);
        }
        public static void VertexAttrib4NARB(uint index, ushort[] v)
        {
            GetDelegateFor<glVertexAttrib4NusvARB>()(index, v);
        }
        public static void VertexAttrib4ARB(uint index, sbyte[] v)
        {
            GetDelegateFor<glVertexAttrib4bvARB>()(index, v);
        }
        public static void VertexAttrib4ARB(uint index, double x, double y, double z, double w)
        {
            GetDelegateFor<glVertexAttrib4dARB>()(index, x, y, z, w);
        }
        public static void VertexAttrib4ARB(uint index, double[] v)
        {
            GetDelegateFor<glVertexAttrib4dvARB>()(index, v);
        }
        public static void VertexAttrib4ARB(uint index, float x, float y, float z, float w)
        {
            GetDelegateFor<glVertexAttrib4fARB>()(index, x, y, z, w);
        }
        public static void VertexAttrib4ARB(uint index, float[] v)
        {
            GetDelegateFor<glVertexAttrib4fvARB>()(index, v);
        }
        public static void VertexAttrib4ARB(uint index, int[] v)
        {
            GetDelegateFor<glVertexAttrib4ivARB>()(index, v);
        }
        public static void VertexAttrib4ARB(uint index, short x, short y, short z, short w)
        {
            GetDelegateFor<glVertexAttrib4sARB>()(index, x, y, z, w);
        }
        public static void VertexAttrib4ARB(uint index, short[] v)
        {
            GetDelegateFor<glVertexAttrib4svARB>()(index, v);
        }
        public static void VertexAttrib4ARB(uint index, byte[] v)
        {
            GetDelegateFor<glVertexAttrib4ubvARB>()(index, v);
        }
        public static void VertexAttrib4ARB(uint index, uint[] v)
        {
            GetDelegateFor<glVertexAttrib4uivARB>()(index, v);
        }
        public static void VertexAttrib4ARB(uint index, ushort[] v)
        {
            GetDelegateFor<glVertexAttrib4usvARB>()(index, v);
        }
        public static void VertexAttribPointerARB(uint index, int size, uint type, bool normalized, int stride, IntPtr pointer)
        {
            GetDelegateFor<glVertexAttribPointerARB>()(index, size, type, normalized, stride, pointer);
        }
        public static void EnableVertexAttribArrayARB(uint index)
        {
            GetDelegateFor<glEnableVertexAttribArrayARB>()(index);
        }
        public static void DisableVertexAttribArrayARB(uint index)
        {
            GetDelegateFor<glDisableVertexAttribArrayARB>()(index);
        }
        public static void ProgramStringARB(uint target, uint format, int len, IntPtr str)
        {
            GetDelegateFor<glProgramStringARB>()(target, format, len, str);
        }
        public static void BindProgramARB(uint target, uint program)
        {
            GetDelegateFor<glBindProgramARB>()(target, program);
        }
        public static void DeleteProgramsARB(int n, uint[] programs)
        {
            GetDelegateFor<glDeleteProgramsARB>()(n, programs);
        }
        public static void GenProgramsARB(int n, uint[] programs)
        {
            GetDelegateFor<glGenProgramsARB>()(n, programs);
        }
        public static void ProgramEnvParameter4ARB(uint target, uint index, double x, double y, double z, double w)
        {
            GetDelegateFor<glProgramEnvParameter4dARB>()(target, index, x, y, z, w);
        }
        public static void ProgramEnvParameter4ARB(uint target, uint index, double[] parameters)
        {
            GetDelegateFor<glProgramEnvParameter4dvARB>()(target, index, parameters);
        }
        public static void ProgramEnvParameter4ARB(uint target, uint index, float x, float y, float z, float w)
        {
            GetDelegateFor<glProgramEnvParameter4fARB>()(target, index, x, y, z, w);
        }
        public static void ProgramEnvParameter4ARB(uint target, uint index, float[] parameters)
        {
            GetDelegateFor<glProgramEnvParameter4fvARB>()(target, index, parameters);
        }
        public static void ProgramLocalParameter4ARB(uint target, uint index, double x, double y, double z, double w)
        {
            GetDelegateFor<glProgramLocalParameter4dARB>()(target, index, x, y, z, w);
        }
        public static void ProgramLocalParameter4ARB(uint target, uint index, double[] parameters)
        {
            GetDelegateFor<glProgramLocalParameter4dvARB>()(target, index, parameters);
        }
        public static void ProgramLocalParameter4ARB(uint target, uint index, float x, float y, float z, float w)
        {
            GetDelegateFor<glProgramLocalParameter4fARB>()(target, index, x, y, z, w);
        }
        public static void ProgramLocalParameter4ARB(uint target, uint index, float[] parameters)
        {
            GetDelegateFor<glProgramLocalParameter4fvARB>()(target, index, parameters);
        }
        public static void GetProgramEnvParameterdARB(uint target, uint index, double[] parameters)
        {
            GetDelegateFor<glGetProgramEnvParameterdvARB>()(target, index, parameters);
        }
        public static void GetProgramEnvParameterfARB(uint target, uint index, float[] parameters)
        {
            GetDelegateFor<glGetProgramEnvParameterfvARB>()(target, index, parameters);
        }
        public static void GetProgramLocalParameterARB(uint target, uint index, double[] parameters)
        {
            GetDelegateFor<glGetProgramLocalParameterdvARB>()(target, index, parameters);
        }
        public static void GetProgramLocalParameterARB(uint target, uint index, float[] parameters)
        {
            GetDelegateFor<glGetProgramLocalParameterfvARB>()(target, index, parameters);
        }
        public static void GetProgramARB(uint target, uint pname, int[] parameters)
        {
            GetDelegateFor<glGetProgramivARB>()(target, pname, parameters);
        }
        public static void GetProgramStringARB(uint target, uint pname, IntPtr str)
        {
            GetDelegateFor<glGetProgramStringARB>()(target, pname, str);
        }
        public static void GetVertexAttribARB(uint index, uint pname, double[] parameters)
        {
            GetDelegateFor<glGetVertexAttribdvARB>()(index, pname, parameters);
        }
        public static void GetVertexAttribARB(uint index, uint pname, float[] parameters)
        {
            GetDelegateFor<glGetVertexAttribfvARB>()(index, pname, parameters);
        }
        public static void GetVertexAttribARB(uint index, uint pname, int[] parameters)
        {
            GetDelegateFor<glGetVertexAttribivARB>()(index, pname, parameters);
        }
        public static void GetVertexAttribPointerARB(uint index, uint pname, IntPtr pointer)
        {
            GetDelegateFor<glGetVertexAttribPointervARB>()(index, pname, pointer);
        }

        //  Delegates
        private delegate void glVertexAttrib1dARB(uint index, double x);
        private delegate void glVertexAttrib1dvARB(uint index, double[] v);
        private delegate void glVertexAttrib1fARB(uint index, float x);
        private delegate void glVertexAttrib1fvARB(uint index, float[] v);
        private delegate void glVertexAttrib1sARB(uint index, short x);
        private delegate void glVertexAttrib1svARB(uint index, short[] v);
        private delegate void glVertexAttrib2dARB(uint index, double x, double y);
        private delegate void glVertexAttrib2dvARB(uint index, double[] v);
        private delegate void glVertexAttrib2fARB(uint index, float x, float y);
        private delegate void glVertexAttrib2fvARB(uint index, float[] v);
        private delegate void glVertexAttrib2sARB(uint index, short x, short y);
        private delegate void glVertexAttrib2svARB(uint index, short[] v);
        private delegate void glVertexAttrib3dARB(uint index, double x, double y, double z);
        private delegate void glVertexAttrib3dvARB(uint index, double[] v);
        private delegate void glVertexAttrib3fARB(uint index, float x, float y, float z);
        private delegate void glVertexAttrib3fvARB(uint index, float[] v);
        private delegate void glVertexAttrib3sARB(uint index, short x, short y, short z);
        private delegate void glVertexAttrib3svARB(uint index, short[] v);
        private delegate void glVertexAttrib4NbvARB(uint index, sbyte[] v);
        private delegate void glVertexAttrib4NivARB(uint index, int[] v);
        private delegate void glVertexAttrib4NsvARB(uint index, short[] v);
        private delegate void glVertexAttrib4NubARB(uint index, byte x, byte y, byte z, byte w);
        private delegate void glVertexAttrib4NubvARB(uint index, byte[] v);
        private delegate void glVertexAttrib4NuivARB(uint index, uint[] v);
        private delegate void glVertexAttrib4NusvARB(uint index, ushort[] v);
        private delegate void glVertexAttrib4bvARB(uint index, sbyte[] v);
        private delegate void glVertexAttrib4dARB(uint index, double x, double y, double z, double w);
        private delegate void glVertexAttrib4dvARB(uint index, double[] v);
        private delegate void glVertexAttrib4fARB(uint index, float x, float y, float z, float w);
        private delegate void glVertexAttrib4fvARB(uint index, float[] v);
        private delegate void glVertexAttrib4ivARB(uint index, int[] v);
        private delegate void glVertexAttrib4sARB(uint index, short x, short y, short z, short w);
        private delegate void glVertexAttrib4svARB(uint index, short[] v);
        private delegate void glVertexAttrib4ubvARB(uint index, byte[] v);
        private delegate void glVertexAttrib4uivARB(uint index, uint[] v);
        private delegate void glVertexAttrib4usvARB(uint index, ushort[] v);
        private delegate void glVertexAttribPointerARB(uint index, int size, uint type, bool normalized, int stride, IntPtr pointer);
        private delegate void glEnableVertexAttribArrayARB(uint index);
        private delegate void glDisableVertexAttribArrayARB(uint index);
        private delegate void glProgramStringARB(uint target, uint format, int len, IntPtr str);
        private delegate void glBindProgramARB(uint target, uint program);
        private delegate void glDeleteProgramsARB(int n, uint[] programs);
        private delegate void glGenProgramsARB(int n, uint[] programs);
        private delegate void glProgramEnvParameter4dARB(uint target, uint index, double x, double y, double z, double w);
        private delegate void glProgramEnvParameter4dvARB(uint target, uint index, double[] parameters);
        private delegate void glProgramEnvParameter4fARB(uint target, uint index, float x, float y, float z, float w);
        private delegate void glProgramEnvParameter4fvARB(uint target, uint index, float[] parameters);
        private delegate void glProgramLocalParameter4dARB(uint target, uint index, double x, double y, double z, double w);
        private delegate void glProgramLocalParameter4dvARB(uint target, uint index, double[] parameters);
        private delegate void glProgramLocalParameter4fARB(uint target, uint index, float x, float y, float z, float w);
        private delegate void glProgramLocalParameter4fvARB(uint target, uint index, float[] parameters);
        private delegate void glGetProgramEnvParameterdvARB(uint target, uint index, double[] parameters);
        private delegate void glGetProgramEnvParameterfvARB(uint target, uint index, float[] parameters);
        private delegate void glGetProgramLocalParameterdvARB(uint target, uint index, double[] parameters);
        private delegate void glGetProgramLocalParameterfvARB(uint target, uint index, float[] parameters);
        private delegate void glGetProgramivARB(uint target, uint pname, int[] parameters);
        private delegate void glGetProgramStringARB(uint target, uint pname, IntPtr str);
        private delegate void glGetVertexAttribdvARB(uint index, uint pname, double[] parameters);
        private delegate void glGetVertexAttribfvARB(uint index, uint pname, float[] parameters);
        private delegate void glGetVertexAttribivARB(uint index, uint pname, int[] parameters);
        private delegate void glGetVertexAttribPointervARB(uint index, uint pname, IntPtr pointer);

        //  Constants
        public const uint GL_COLOR_SUM_ARB = 0x8458;
        public const uint GL_VERTEX_PROGRAM_ARB = 0x8620;
        public const uint GL_VERTEX_ATTRIB_ARRAY_ENABLED_ARB = 0x8622;
        public const uint GL_VERTEX_ATTRIB_ARRAY_SIZE_ARB = 0x8623;
        public const uint GL_VERTEX_ATTRIB_ARRAY_STRIDE_ARB = 0x8624;
        public const uint GL_VERTEX_ATTRIB_ARRAY_TYPE_ARB = 0x8625;
        public const uint GL_CURRENT_VERTEX_ATTRIB_ARB = 0x8626;
        public const uint GL_PROGRAM_LENGTH_ARB = 0x8627;
        public const uint GL_PROGRAM_STRING_ARB = 0x8628;
        public const uint GL_MAX_PROGRAM_MATRIX_STACK_DEPTH_ARB = 0x862E;
        public const uint GL_MAX_PROGRAM_MATRICES_ARB = 0x862F;
        public const uint GL_CURRENT_MATRIX_STACK_DEPTH_ARB = 0x8640;
        public const uint GL_CURRENT_MATRIX_ARB = 0x8641;
        public const uint GL_VERTEX_PROGRAM_POINT_SIZE_ARB = 0x8642;
        public const uint GL_VERTEX_PROGRAM_TWO_SIDE_ARB = 0x8643;
        public const uint GL_VERTEX_ATTRIB_ARRAY_POINTER_ARB = 0x8645;
        public const uint GL_PROGRAM_ERROR_POSITION_ARB = 0x864B;
        public const uint GL_PROGRAM_BINDING_ARB = 0x8677;
        public const uint GL_MAX_VERTEX_ATTRIBS_ARB = 0x8869;
        public const uint GL_VERTEX_ATTRIB_ARRAY_NORMALIZED_ARB = 0x886A;
        public const uint GL_PROGRAM_ERROR_STRING_ARB = 0x8874;
        public const uint GL_PROGRAM_FORMAT_ASCII_ARB = 0x8875;
        public const uint GL_PROGRAM_FORMAT_ARB = 0x8876;
        public const uint GL_PROGRAM_INSTRUCTIONS_ARB = 0x88A0;
        public const uint GL_MAX_PROGRAM_INSTRUCTIONS_ARB = 0x88A1;
        public const uint GL_PROGRAM_NATIVE_INSTRUCTIONS_ARB = 0x88A2;
        public const uint GL_MAX_PROGRAM_NATIVE_INSTRUCTIONS_ARB = 0x88A3;
        public const uint GL_PROGRAM_TEMPORARIES_ARB = 0x88A4;
        public const uint GL_MAX_PROGRAM_TEMPORARIES_ARB = 0x88A5;
        public const uint GL_PROGRAM_NATIVE_TEMPORARIES_ARB = 0x88A6;
        public const uint GL_MAX_PROGRAM_NATIVE_TEMPORARIES_ARB = 0x88A7;
        public const uint GL_PROGRAM_PARAMETERS_ARB = 0x88A8;
        public const uint GL_MAX_PROGRAM_PARAMETERS_ARB = 0x88A9;
        public const uint GL_PROGRAM_NATIVE_PARAMETERS_ARB = 0x88AA;
        public const uint GL_MAX_PROGRAM_NATIVE_PARAMETERS_ARB = 0x88AB;
        public const uint GL_PROGRAM_ATTRIBS_ARB = 0x88AC;
        public const uint GL_MAX_PROGRAM_ATTRIBS_ARB = 0x88AD;
        public const uint GL_PROGRAM_NATIVE_ATTRIBS_ARB = 0x88AE;
        public const uint GL_MAX_PROGRAM_NATIVE_ATTRIBS_ARB = 0x88AF;
        public const uint GL_PROGRAM_ADDRESS_REGISTERS_ARB = 0x88B0;
        public const uint GL_MAX_PROGRAM_ADDRESS_REGISTERS_ARB = 0x88B1;
        public const uint GL_PROGRAM_NATIVE_ADDRESS_REGISTERS_ARB = 0x88B2;
        public const uint GL_MAX_PROGRAM_NATIVE_ADDRESS_REGISTERS_ARB = 0x88B3;
        public const uint GL_MAX_PROGRAM_LOCAL_PARAMETERS_ARB = 0x88B4;
        public const uint GL_MAX_PROGRAM_ENV_PARAMETERS_ARB = 0x88B5;
        public const uint GL_PROGRAM_UNDER_NATIVE_LIMITS_ARB = 0x88B6;
        public const uint GL_TRANSPOSE_CURRENT_MATRIX_ARB = 0x88B7;
        public const uint GL_MATRIX0_ARB = 0x88C0;
        public const uint GL_MATRIX1_ARB = 0x88C1;
        public const uint GL_MATRIX2_ARB = 0x88C2;
        public const uint GL_MATRIX3_ARB = 0x88C3;
        public const uint GL_MATRIX4_ARB = 0x88C4;
        public const uint GL_MATRIX5_ARB = 0x88C5;
        public const uint GL_MATRIX6_ARB = 0x88C6;
        public const uint GL_MATRIX7_ARB = 0x88C7;
        public const uint GL_MATRIX8_ARB = 0x88C8;
        public const uint GL_MATRIX9_ARB = 0x88C9;
        public const uint GL_MATRIX10_ARB = 0x88CA;
        public const uint GL_MATRIX11_ARB = 0x88CB;
        public const uint GL_MATRIX12_ARB = 0x88CC;
        public const uint GL_MATRIX13_ARB = 0x88CD;
        public const uint GL_MATRIX14_ARB = 0x88CE;
        public const uint GL_MATRIX15_ARB = 0x88CF;
        public const uint GL_MATRIX16_ARB = 0x88D0;
        public const uint GL_MATRIX17_ARB = 0x88D1;
        public const uint GL_MATRIX18_ARB = 0x88D2;
        public const uint GL_MATRIX19_ARB = 0x88D3;
        public const uint GL_MATRIX20_ARB = 0x88D4;
        public const uint GL_MATRIX21_ARB = 0x88D5;
        public const uint GL_MATRIX22_ARB = 0x88D6;
        public const uint GL_MATRIX23_ARB = 0x88D7;
        public const uint GL_MATRIX24_ARB = 0x88D8;
        public const uint GL_MATRIX25_ARB = 0x88D9;
        public const uint GL_MATRIX26_ARB = 0x88DA;
        public const uint GL_MATRIX27_ARB = 0x88DB;
        public const uint GL_MATRIX28_ARB = 0x88DC;
        public const uint GL_MATRIX29_ARB = 0x88DD;
        public const uint GL_MATRIX30_ARB = 0x88DE;
        public const uint GL_MATRIX31_ARB = 0x88DF;

        #endregion

        #region GL_ARB_vertex_shader

        //  Methods
        public static void BindAttribLocationARB(uint programObj, uint index, string name)
        {
            GetDelegateFor<glBindAttribLocationARB>()(programObj, index, name);
        }
        public static void GetActiveAttribARB(uint programObj, uint index, int maxLength, int[] length, int[] size, uint[] type, string name)
        {
            GetDelegateFor<glGetActiveAttribARB>()(programObj, index, maxLength, length, size, type, name);
        }
        public static uint GetAttribLocationARB(uint programObj, string name)
        {
            return (uint)GetDelegateFor<glGetAttribLocationARB>()(programObj, name);
        }

        //  Delegates
        private delegate void glBindAttribLocationARB(uint programObj, uint index, string name);
        private delegate void glGetActiveAttribARB(uint programObj, uint index, int maxLength, int[] length, int[] size, uint[] type, string name);
        private delegate uint glGetAttribLocationARB(uint programObj, string name);

        //  Constants
        public const uint GL_VERTEX_SHADER_ARB = 0x8B31;
        public const uint GL_MAX_VERTEX_UNIFORM_COMPONENTS_ARB = 0x8B4A;
        public const uint GL_MAX_VARYING_FLOATS_ARB = 0x8B4B;
        public const uint GL_MAX_VERTEX_TEXTURE_IMAGE_UNITS_ARB = 0x8B4C;
        public const uint GL_MAX_COMBINED_TEXTURE_IMAGE_UNITS_ARB = 0x8B4D;
        public const uint GL_OBJECT_ACTIVE_ATTRIBUTES_ARB = 0x8B89;
        public const uint GL_OBJECT_ACTIVE_ATTRIBUTE_MAX_LENGTH_ARB = 0x8B8A;

        #endregion

        #region GL_ARB_fragment_shader

        public const uint GL_FRAGMENT_SHADER_ARB = 0x8B30;
        public const uint GL_MAX_FRAGMENT_UNIFORM_COMPONENTS_ARB = 0x8B49;
        public const uint GL_FRAGMENT_SHADER_DERIVATIVE_HINT_ARB = 0x8B8B;

        #endregion

        #region GL_ARB_draw_buffers

        //  Methods
        public static void DrawBuffersARB(int n, uint[] bufs)
        {
            GetDelegateFor<glDrawBuffersARB>()(n, bufs);
        }

        //  Delegates
        private delegate void glDrawBuffersARB(int n, uint[] bufs);

        //  Constants        
        public const uint GL_MAX_DRAW_BUFFERS_ARB = 0x8824;
        public const uint GL_DRAW_BUFFER0_ARB = 0x8825;
        public const uint GL_DRAW_BUFFER1_ARB = 0x8826;
        public const uint GL_DRAW_BUFFER2_ARB = 0x8827;
        public const uint GL_DRAW_BUFFER3_ARB = 0x8828;
        public const uint GL_DRAW_BUFFER4_ARB = 0x8829;
        public const uint GL_DRAW_BUFFER5_ARB = 0x882A;
        public const uint GL_DRAW_BUFFER6_ARB = 0x882B;
        public const uint GL_DRAW_BUFFER7_ARB = 0x882C;
        public const uint GL_DRAW_BUFFER8_ARB = 0x882D;
        public const uint GL_DRAW_BUFFER9_ARB = 0x882E;
        public const uint GL_DRAW_BUFFER10_ARB = 0x882F;
        public const uint GL_DRAW_BUFFER11_ARB = 0x8830;
        public const uint GL_DRAW_BUFFER12_ARB = 0x8831;
        public const uint GL_DRAW_BUFFER13_ARB = 0x8832;
        public const uint GL_DRAW_BUFFER14_ARB = 0x8833;
        public const uint GL_DRAW_BUFFER15_ARB = 0x8834;

        #endregion

        #region GL_ARB_texture_non_power_of_two

        //  No methods or constants

        #endregion

        #region GL_ARB_texture_rectangle

        //  Constants
        public const uint GL_TEXTURE_RECTANGLE_ARB = 0x84F5;
        public const uint GL_TEXTURE_BINDING_RECTANGLE_ARB = 0x84F6;
        public const uint GL_PROXY_TEXTURE_RECTANGLE_ARB = 0x84F7;
        public const uint GL_MAX_RECTANGLE_TEXTURE_SIZE_ARB = 0x84F8;

        #endregion

        #region GL_ARB_point_sprite

        //  Constants
        public const uint GL_POINT_SPRITE_ARB = 0x8861;
        public const uint GL_COORD_REPLACE_ARB = 0x8862;

        #endregion

        #region GL_ARB_texture_float

        //  Constants
        public const uint GL_TEXTURE_RED_TYPE_ARB = 0x8C10;
        public const uint GL_TEXTURE_GREEN_TYPE_ARB = 0x8C11;
        public const uint GL_TEXTURE_BLUE_TYPE_ARB = 0x8C12;
        public const uint GL_TEXTURE_ALPHA_TYPE_ARB = 0x8C13;
        public const uint GL_TEXTURE_LUMINANCE_TYPE_ARB = 0x8C14;
        public const uint GL_TEXTURE_INTENSITY_TYPE_ARB = 0x8C15;
        public const uint GL_TEXTURE_DEPTH_TYPE_ARB = 0x8C16;
        public const uint GL_UNSIGNED_NORMALIZED_ARB = 0x8C17;
        public const uint GL_RGBA32F_ARB = 0x8814;
        public const uint GL_RGB32F_ARB = 0x8815;
        public const uint GL_ALPHA32F_ARB = 0x8816;
        public const uint GL_INTENSITY32F_ARB = 0x8817;
        public const uint GL_LUMINANCE32F_ARB = 0x8818;
        public const uint GL_LUMINANCE_ALPHA32F_ARB = 0x8819;
        public const uint GL_RGBA16F_ARB = 0x881A;
        public const uint GL_RGB16F_ARB = 0x881B;
        public const uint GL_ALPHA16F_ARB = 0x881C;
        public const uint GL_INTENSITY16F_ARB = 0x881D;
        public const uint GL_LUMINANCE16F_ARB = 0x881E;
        public const uint GL_LUMINANCE_ALPHA16F_ARB = 0x881F;

        #endregion

        #region GL_EXT_blend_equation_separate

        //  Methods
        public static void BlendEquationSeparateEXT(uint modeRGB, uint modeAlpha)
        {
            // GetDelegateFor<glBlendEquationEXT>()(modeRGB, modeAlpha);
            GetDelegateFor<glBlendEquationEXT>()(modeRGB);
        }

        //  Delegates
        private delegate void glBlendEquationSeparateEXT(uint modeRGB, uint modeAlpha);

        //  Constants
        public const uint GL_BLEND_EQUATION_RGB_EXT = 0x8009;
        public const uint GL_BLEND_EQUATION_ALPHA_EXT = 0x883D;

        #endregion

        #region GL_EXT_stencil_two_side

        //  Methods
        public static void ActiveStencilFaceEXT(uint face)
        {
            GetDelegateFor<glActiveStencilFaceEXT>()(face);
        }

        //  Delegates
        private delegate void glActiveStencilFaceEXT(uint face);

        //  Constants
        public const uint GL_STENCIL_TEST_TWO_SIDE_EXT = 0x8009;
        public const uint GL_ACTIVE_STENCIL_FACE_EXT = 0x883D;

        #endregion

        #region GL_ARB_pixel_buffer_object

        public const uint GL_PIXEL_PACK_BUFFER_ARB = 0x88EB;
        public const uint GL_PIXEL_UNPACK_BUFFER_ARB = 0x88EC;
        public const uint GL_PIXEL_PACK_BUFFER_BINDING_ARB = 0x88ED;
        public const uint GL_PIXEL_UNPACK_BUFFER_BINDING_ARB = 0x88EF;

        #endregion

        #region GL_EXT_texture_sRGB

        public const uint GL_SRGB_EXT = 0x8C40;
        public const uint GL_SRGB8_EXT = 0x8C41;
        public const uint GL_SRGB_ALPHA_EXT = 0x8C42;
        public const uint GL_SRGB8_ALPHA8_EXT = 0x8C43;
        public const uint GL_SLUMINANCE_ALPHA_EXT = 0x8C44;
        public const uint GL_SLUMINANCE8_ALPHA8_EXT = 0x8C45;
        public const uint GL_SLUMINANCE_EXT = 0x8C46;
        public const uint GL_SLUMINANCE8_EXT = 0x8C47;
        public const uint GL_COMPRESSED_SRGB_EXT = 0x8C48;
        public const uint GL_COMPRESSED_SRGB_ALPHA_EXT = 0x8C49;
        public const uint GL_COMPRESSED_SLUMINANCE_EXT = 0x8C4A;
        public const uint GL_COMPRESSED_SLUMINANCE_ALPHA_EXT = 0x8C4B;
        public const uint GL_COMPRESSED_SRGB_S3TC_DXT1_EXT = 0x8C4C;
        public const uint GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT1_EXT = 0x8C4D;
        public const uint GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT3_EXT = 0x8C4E;
        public const uint GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT5_EXT = 0x8C4F;

        #endregion

        #region GL_EXT_framebuffer_object

        //  Methods
        public static bool IsRenderbufferEXT(uint renderbuffer)
        {
            return (bool)GetDelegateFor<glIsRenderbufferEXT>()(renderbuffer);
        }

        public static void BindRenderbufferEXT(uint target, uint renderbuffer)
        {
            GetDelegateFor<glBindRenderbufferEXT>()(target, renderbuffer);
        }

        public static void DeleteRenderbuffersEXT(uint n, uint[] renderbuffers)
        {
            GetDelegateFor<glDeleteRenderbuffersEXT>()(n, renderbuffers);
        }

        public static void GenRenderbuffersEXT(uint n, uint[] renderbuffers)
        {
            GetDelegateFor<glGenRenderbuffersEXT>()(n, renderbuffers);
        }

        public static void RenderbufferStorageEXT(uint target, uint internalformat, int width, int height)
        {
            GetDelegateFor<glRenderbufferStorageEXT>()(target, internalformat, width, height);
        }

        public static void GetRenderbufferParameterivEXT(uint target, uint pname, int[] parameters)
        {
            GetDelegateFor<glGetRenderbufferParameterivEXT>()(target, pname, parameters);
        }

        public static bool IsFramebufferEXT(uint framebuffer)
        {
            return (bool)GetDelegateFor<glIsFramebufferEXT>()(framebuffer);
        }

        public static void BindFramebufferEXT(uint target, uint framebuffer)
        {
            GetDelegateFor<glBindFramebufferEXT>()(target, framebuffer);
        }
        
        public static void DeleteFramebuffer(uint framebuffer)
        {
            GetDelegateFor<glDeleteFramebuffers>()(1, new[] { framebuffer });
        }
        public static void DeleteFramebuffers(uint n, uint[] framebuffers)
        {
            GetDelegateFor<glDeleteFramebuffers>()(n, framebuffers);
        }
        public static void DeleteFramebufferEXT(uint framebuffer)
        {
            GetDelegateFor<glDeleteFramebuffersEXT>()(1, new[] { framebuffer });
        }
        public static void DeleteFramebuffersEXT(uint n, uint[] framebuffers)
        {
            GetDelegateFor<glDeleteFramebuffersEXT>()(n, framebuffers);
        }
        
        public static uint GenFramebuffer()
        {
            uint[] framebuffers = new uint[1];
            GetDelegateFor<glGenFramebuffers>()(1, framebuffers);
            return framebuffers[0];
        }
        public static void GenFramebuffers(uint n, uint[] framebuffers)
        {
            GetDelegateFor<glGenFramebuffers>()(n, framebuffers);
        }
        public static uint GenFramebufferEXT()
        {
            uint[] framebuffers = new uint[1];
            GetDelegateFor<glGenFramebuffersEXT>()(1, framebuffers);
            return framebuffers[0];
        }
        public static void GenFramebuffersEXT(uint n, uint[] framebuffers)
        {
            GetDelegateFor<glGenFramebuffersEXT>()(n, framebuffers);
        }

        public static uint CheckFramebufferStatusEXT(uint target)
        {
            return (uint)GetDelegateFor<glCheckFramebufferStatusEXT>()(target);
        }

        public static void FramebufferTexture1DEXT(uint target, uint attachment, uint textarget, uint texture, int level)
        {
            GetDelegateFor<glFramebufferTexture1DEXT>()(target, attachment, textarget, texture, level);
        }

        public static void FramebufferTexture2DEXT(uint target, uint attachment, uint textarget, uint texture, int level)
        {
            GetDelegateFor<glFramebufferTexture2DEXT>()(target, attachment, textarget, texture, level);
        }

        public static void FramebufferTexture3DEXT(uint target, uint attachment, uint textarget, uint texture, int level, int zoffset)
        {
            GetDelegateFor<glFramebufferTexture3DEXT>()(target, attachment, textarget, texture, level, zoffset);
        }

        public static void FramebufferRenderbufferEXT(uint target, uint attachment, uint renderbuffertarget, uint renderbuffer)
        {
            GetDelegateFor<glFramebufferRenderbufferEXT>()(target, attachment, renderbuffertarget, renderbuffer);
        }

        public static void GetFramebufferAttachmentParameterivEXT(uint target, uint attachment, uint pname, int[] parameters)
        {
            GetDelegateFor<glGetFramebufferAttachmentParameterivEXT>()(target, attachment, pname, parameters);
        }
        
        public static void GenerateMipmap(uint target)
        {
            GetDelegateFor<glGenerateMipmap>()(target);
        }
        public static void GenerateMipmapEXT(uint target)
        {
            GetDelegateFor<glGenerateMipmapEXT>()(target);
        }
        
        public static void GenerateTextureMipmap(uint handle)
        {
            GetDelegateFor<glGenerateTextureMipmap>()(handle);
        }
        public static void GenerateTextureMipmapEXT(uint handle, uint target)
        {
            GetDelegateFor<glGenerateTextureMipmapEXT>()(handle, target);
        }

        //  Delegates
        private delegate bool glIsRenderbufferEXT(uint renderbuffer);
        private delegate void glBindRenderbufferEXT(uint target, uint renderbuffer);
        private delegate void glDeleteRenderbuffersEXT(uint n, uint[] renderbuffers);
        private delegate void glGenRenderbuffersEXT(uint n, uint[] renderbuffers);
        private delegate void glRenderbufferStorageEXT(uint target, uint internalformat, int width, int height);
        private delegate void glGetRenderbufferParameterivEXT(uint target, uint pname, int[] parameters);
        private delegate bool glIsFramebufferEXT(uint framebuffer);
        private delegate void glBindFramebufferEXT(uint target, uint framebuffer);
        private delegate void glDeleteFramebuffers(uint n, uint[] framebuffers);
        private delegate void glDeleteFramebuffersEXT(uint n, uint[] framebuffers);
        private delegate void glGenFramebuffers(uint n, uint[] framebuffers);
        private delegate void glGenFramebuffersEXT(uint n, uint[] framebuffers);
        private delegate uint glCheckFramebufferStatusEXT(uint target);
        private delegate void glFramebufferTexture1DEXT(uint target, uint attachment, uint textarget, uint texture, int level);
        private delegate void glFramebufferTexture2DEXT(uint target, uint attachment, uint textarget, uint texture, int level);
        private delegate void glFramebufferTexture3DEXT(uint target, uint attachment, uint textarget, uint texture, int level, int zoffset);
        private delegate void glFramebufferRenderbufferEXT(uint target, uint attachment, uint renderbuffertarget, uint renderbuffer);
        private delegate void glGetFramebufferAttachmentParameterivEXT(uint target, uint attachment, uint pname, int[] parameters);
        private delegate void glGenerateMipmap(uint target);
        private delegate void glGenerateMipmapEXT(uint target);
        private delegate void glGenerateTextureMipmap(uint handle);
        private delegate void glGenerateTextureMipmapEXT(uint handle, uint target);

        //  Constants
        public const uint GL_INVALID_FRAMEBUFFER_OPERATION_EXT = 0x0506;
        public const uint GL_MAX_RENDERBUFFER_SIZE_EXT = 0x84E8;
        public const uint GL_FRAMEBUFFER_BINDING_EXT = 0x8CA6;
        public const uint GL_RENDERBUFFER_BINDING_EXT = 0x8CA7;
        public const uint GL_FRAMEBUFFER_ATTACHMENT_OBJECT_TYPE_EXT = 0x8CD0;
        public const uint GL_FRAMEBUFFER_ATTACHMENT_OBJECT_NAME_EXT = 0x8CD1;
        public const uint GL_FRAMEBUFFER_ATTACHMENT_TEXTURE_LEVEL_EXT = 0x8CD2;
        public const uint GL_FRAMEBUFFER_ATTACHMENT_TEXTURE_CUBE_MAP_FACE_EXT = 0x8CD3;
        public const uint GL_FRAMEBUFFER_ATTACHMENT_TEXTURE_3D_ZOFFSET_EXT = 0x8CD4;
        public const uint GL_FRAMEBUFFER_COMPLETE_EXT = 0x8CD5;
        public const uint GL_FRAMEBUFFER_INCOMPLETE_ATTACHMENT_EXT = 0x8CD6;
        public const uint GL_FRAMEBUFFER_INCOMPLETE_MISSING_ATTACHMENT_EXT = 0x8CD7;
        public const uint GL_FRAMEBUFFER_INCOMPLETE_DIMENSIONS_EXT = 0x8CD9;
        public const uint GL_FRAMEBUFFER_INCOMPLETE_FORMATS_EXT = 0x8CDA;
        public const uint GL_FRAMEBUFFER_INCOMPLETE_DRAW_BUFFER_EXT = 0x8CDB;
        public const uint GL_FRAMEBUFFER_INCOMPLETE_READ_BUFFER_EXT = 0x8CDC;
        public const uint GL_FRAMEBUFFER_UNSUPPORTED_EXT = 0x8CDD;
        public const uint GL_MAX_COLOR_ATTACHMENTS_EXT = 0x8CDF;
        public const uint GL_COLOR_ATTACHMENT0_EXT = 0x8CE0;
        public const uint GL_COLOR_ATTACHMENT1_EXT = 0x8CE1;
        public const uint GL_COLOR_ATTACHMENT2_EXT = 0x8CE2;
        public const uint GL_COLOR_ATTACHMENT3_EXT = 0x8CE3;
        public const uint GL_COLOR_ATTACHMENT4_EXT = 0x8CE4;
        public const uint GL_COLOR_ATTACHMENT5_EXT = 0x8CE5;
        public const uint GL_COLOR_ATTACHMENT6_EXT = 0x8CE6;
        public const uint GL_COLOR_ATTACHMENT7_EXT = 0x8CE7;
        public const uint GL_COLOR_ATTACHMENT8_EXT = 0x8CE8;
        public const uint GL_COLOR_ATTACHMENT9_EXT = 0x8CE9;
        public const uint GL_COLOR_ATTACHMENT10_EXT = 0x8CEA;
        public const uint GL_COLOR_ATTACHMENT11_EXT = 0x8CEB;
        public const uint GL_COLOR_ATTACHMENT12_EXT = 0x8CEC;
        public const uint GL_COLOR_ATTACHMENT13_EXT = 0x8CED;
        public const uint GL_COLOR_ATTACHMENT14_EXT = 0x8CEE;
        public const uint GL_COLOR_ATTACHMENT15_EXT = 0x8CEF;
        public const uint GL_DEPTH_ATTACHMENT_EXT = 0x8D00;
        public const uint GL_STENCIL_ATTACHMENT_EXT = 0x8D20;
        public const uint GL_FRAMEBUFFER_EXT = 0x8D40;
        public const uint GL_RENDERBUFFER_EXT = 0x8D41;
        public const uint GL_RENDERBUFFER_WIDTH_EXT = 0x8D42;
        public const uint GL_RENDERBUFFER_HEIGHT_EXT = 0x8D43;
        public const uint GL_RENDERBUFFER_INTERNAL_FORMAT_EXT = 0x8D44;
        public const uint GL_STENCIL_INDEX1_EXT = 0x8D46;
        public const uint GL_STENCIL_INDEX4_EXT = 0x8D47;
        public const uint GL_STENCIL_INDEX8_EXT = 0x8D48;
        public const uint GL_STENCIL_INDEX16_EXT = 0x8D49;
        public const uint GL_RENDERBUFFER_RED_SIZE_EXT = 0x8D50;
        public const uint GL_RENDERBUFFER_GREEN_SIZE_EXT = 0x8D51;
        public const uint GL_RENDERBUFFER_BLUE_SIZE_EXT = 0x8D52;
        public const uint GL_RENDERBUFFER_ALPHA_SIZE_EXT = 0x8D53;
        public const uint GL_RENDERBUFFER_DEPTH_SIZE_EXT = 0x8D54;
        public const uint GL_RENDERBUFFER_STENCIL_SIZE_EXT = 0x8D55;

        #endregion

        #region GL_EXT_framebuffer_multisample

        //  Methods
        public static void RenderbufferStorageMultisampleEXT(uint target, int samples, uint internalformat, int width, int height)
        {
            GetDelegateFor<glRenderbufferStorageMultisampleEXT>()(target, samples, internalformat, width, height);
        }

        //  Delegates
        private delegate void glRenderbufferStorageMultisampleEXT(uint target, int samples, uint internalformat, int width, int height);

        //  Constants
        public const uint GL_RENDERBUFFER_SAMPLES_EXT = 0x8CAB;
        public const uint GL_FRAMEBUFFER_INCOMPLETE_MULTISAMPLE_EXT = 0x8D56;
        public const uint GL_MAX_SAMPLES_EXT = 0x8D57;

        #endregion

        #region GL_EXT_draw_instanced

        //  Methods
        public static void DrawArraysInstancedEXT(uint mode, int start, int count, int primcount)
        {
            GetDelegateFor<glDrawArraysInstancedEXT>()(mode, start, count, primcount);
        }
        public static void DrawElementsInstancedEXT(uint mode, int count, uint type, IntPtr indices, int primcount)
        {
            GetDelegateFor<glDrawElementsInstancedEXT>()(mode, count, type, indices, primcount);
        }

        //  Delegates
        private delegate void glDrawArraysInstancedEXT(uint mode, int start, int count, int primcount);
        private delegate void glDrawElementsInstancedEXT(uint mode, int count, uint type, IntPtr indices, int primcount);

        #endregion

        #region GL_ARB_vertex_array_object

        //  Methods
        private static readonly Stack<uint> vertexArrayHandles = new Stack<uint>();
        public static void PushVertexArray(uint array)
        {
            vertexArrayHandles.Push(array);
            BindVertexArray(array);
        }
        public static void PopVertexArray()
        {
            vertexArrayHandles.Pop();
            if (vertexArrayHandles.Count == 0)
                BindVertexArray(0);
            else BindVertexArray(vertexArrayHandles.Peek());
        }
        public static void BindVertexArray(uint array)
        {
            GetDelegateFor<glBindVertexArray>()(array);
        }
        public static void DeleteVertexArray(uint array)
        {
            singleHandle[0] = array;
            DeleteVertexArrays(1, singleHandle);
        }
        public static void DeleteVertexArrays(int n, uint[] arrays)
        {
            GetDelegateFor<glDeleteVertexArrays>()(n, arrays);
        }
        public static uint GenVertexArray()
        {
            GenVertexArrays(1, singleHandle);
            return singleHandle[0];
        }
        public static uint[] GenVertexArrays(int n)
        {
            uint[] result = new uint[n];
            GenVertexArrays(n, result);
            return result;
        }
        public static void GenVertexArrays(int n, uint[] arrays)
        {
            GetDelegateFor<glGenVertexArrays>()(n, arrays);
        }
        public static bool IsVertexArray(uint array)
        {
            return (bool)GetDelegateFor<glIsVertexArray>()(array);
        }

        //  Delegates
        private delegate void glBindVertexArray(uint array);
        private delegate void glDeleteVertexArrays(int n, uint[] arrays);
        private delegate void glGenVertexArrays(int n, uint[] arrays);
        private delegate bool glIsVertexArray(uint array);

        //  Constants
        public const uint GL_VERTEX_ARRAY_BINDING = 0x85B5;

        #endregion

        #region GL_EXT_framebuffer_sRGB

        //  Constants
        public const uint GL_FRAMEBUFFER_SRGB_EXT = 0x8DB9;
        public const uint GL_FRAMEBUFFER_SRGB_CAPABLE_EXT = 0x8DBA;

        #endregion

        #region GGL_EXT_transform_feedback

        //  Methods
        public static void BeginTransformFeedbackEXT(uint primitiveMode)
        {
            GetDelegateFor<glBeginTransformFeedbackEXT>()(primitiveMode);
        }
        public static void EndTransformFeedbackEXT()
        {
            GetDelegateFor<glEndTransformFeedbackEXT>()();
        }
        public static void BindBufferRangeEXT(uint target, uint index, uint buffer, int offset, int size)
        {
            GetDelegateFor<glBindBufferRangeEXT>()(target, index, buffer, offset, size);
        }
        public static void BindBufferOffsetEXT(uint target, uint index, uint buffer, int offset)
        {
            GetDelegateFor<glBindBufferOffsetEXT>()(target, index, buffer, offset);
        }
        public static void BindBufferBaseEXT(uint target, uint index, uint buffer)
        {
            GetDelegateFor<glBindBufferBaseEXT>()(target, index, buffer);
        }
        public static void TransformFeedbackVaryingsEXT(uint program, int count, string[] varyings, uint bufferMode)
        {
            GetDelegateFor<glTransformFeedbackVaryingsEXT>()(program, count, varyings, bufferMode);
        }
        public static void GetTransformFeedbackVaryingEXT(uint program, uint index, int bufSize, int[] length, int[] size, uint[] type, string name)
        {
            GetDelegateFor<glGetTransformFeedbackVaryingEXT>()(program, index, bufSize, length, size, type, name);
        }

        //  Delegates
        private delegate void glBeginTransformFeedbackEXT(uint primitiveMode);
        private delegate void glEndTransformFeedbackEXT();
        private delegate void glBindBufferRangeEXT(uint target, uint index, uint buffer, int offset, int size);
        private delegate void glBindBufferOffsetEXT(uint target, uint index, uint buffer, int offset);
        private delegate void glBindBufferBaseEXT(uint target, uint index, uint buffer);
        private delegate void glTransformFeedbackVaryingsEXT(uint program, int count, string[] varyings, uint bufferMode);
        private delegate void glGetTransformFeedbackVaryingEXT(uint program, uint index, int bufSize, int[] length, int[] size, uint[] type, string name);

        //  Constants
        public const uint GL_TRANSFORM_FEEDBACK_BUFFER_EXT = 0x8C8E;
        public const uint GL_TRANSFORM_FEEDBACK_BUFFER_START_EXT = 0x8C84;
        public const uint GL_TRANSFORM_FEEDBACK_BUFFER_SIZE_EXT = 0x8C85;
        public const uint GL_TRANSFORM_FEEDBACK_BUFFER_BINDING_EXT = 0x8C8F;
        public const uint GL_INTERLEAVED_ATTRIBS_EXT = 0x8C8C;
        public const uint GL_SEPARATE_ATTRIBS_EXT = 0x8C8D;
        public const uint GL_PRIMITIVES_GENERATED_EXT = 0x8C87;
        public const uint GL_TRANSFORM_FEEDBACK_PRIMITIVES_WRITTEN_EXT = 0x8C88;
        public const uint GL_RASTERIZER_DISCARD_EXT = 0x8C89;
        public const uint GL_MAX_TRANSFORM_FEEDBACK_INTERLEAVED_COMPONENTS_EXT = 0x8C8A;
        public const uint GL_MAX_TRANSFORM_FEEDBACK_SEPARATE_ATTRIBS_EXT = 0x8C8B;
        public const uint GL_MAX_TRANSFORM_FEEDBACK_SEPARATE_COMPONENTS_EXT = 0x8C80;
        public const uint GL_TRANSFORM_FEEDBACK_VARYINGS_EXT = 0x8C83;
        public const uint GL_TRANSFORM_FEEDBACK_BUFFER_MODE_EXT = 0x8C7F;
        public const uint GL_TRANSFORM_FEEDBACK_VARYING_MAX_LENGTH_EXT = 0x8C76;

        #endregion

        #region WGL_ARB_extensions_string

        /// <summary>
        /// Gets the ARB extensions string.
        /// </summary>
        
        /*
        public string GetExtensionsStringARB()
        {
            return (string)GetDelegateFor<wglGetExtensionsStringARB>()(RenderContextProvider.DeviceContextHandle);
        }
        */

        //  Delegates
        private delegate string wglGetExtensionsStringARB(IntPtr hdc);

        #endregion

        #region WGL_ARB_create_context

        //  Methods

        /// <summary>
        /// Creates a render context with the specified attributes.
        /// </summary>
        /// <param name="hShareContext">
        /// If is not null, then all shareable data (excluding
        /// OpenGL texture objects named 0) will be shared by <hshareContext>,
        /// all other contexts <hshareContext> already shares with, and the
        /// newly created context. An arbitrary number of contexts can share
        /// data in this fashion.</param>
        /// <param name="attribList">
        /// specifies a list of attributes for the context. The
        /// list consists of a sequence of <name,value> pairs terminated by the
        /// value 0. If an attribute is not specified in <attribList>, then the
        /// default value specified below is used instead. If an attribute is
        /// specified more than once, then the last value specified is used.
        /// </param>
        
        /*
        public static IntPtr CreateContextAttribsARB(IntPtr hShareContext, int[] attribList)
        {
            return (IntPtr)GetDelegateFor<wglCreateContextAttribsARB>()(RenderContextProvider.DeviceContextHandle, hShareContext, attribList);
        }
        */

        //  Delegates
        private delegate IntPtr wglCreateContextAttribsARB(IntPtr hDC, IntPtr hShareContext, int[] attribList);

        //  Constants
        public const int WGL_CONTEXT_MAJOR_VERSION_ARB = 0x2091;
        public const int WGL_CONTEXT_MINOR_VERSION_ARB = 0x2092;
        public const int WGL_CONTEXT_LAYER_PLANE_ARB = 0x2093;
        public const int WGL_CONTEXT_FLAGS_ARB = 0x2094;
        public const int WGL_CONTEXT_PROFILE_MASK_ARB = 0x9126;
        public const int WGL_CONTEXT_DEBUG_BIT_ARB = 0x0001;
        public const int WGL_CONTEXT_FORWARD_COMPATIBLE_BIT_ARB = 0x0002;
        public const int WGL_CONTEXT_CORE_PROFILE_BIT_ARB = 0x00000001;
        public const int WGL_CONTEXT_COMPATIBILITY_PROFILE_BIT_ARB = 0x00000002;
        public const int ERROR_INVALID_VERSION_ARB = 0x2095;
        public const int ERROR_INVALID_PROFILE_ARB = 0x2096;

        #endregion

        #region GL_ARB_explicit_uniform_location

        //  Constants

        /// <summary>
        /// The number of available pre-assigned uniform locations to that can default be 
        /// allocated in the default uniform block.
        /// </summary>
        public const int GL_MAX_UNIFORM_LOCATIONS = 0x826E;

        #endregion

        #region GL_ARB_clear_buffer_object

        /// <summary>
        /// Fill a buffer object's data store with a fixed value
        /// </summary>
        /// <param name="target">Specifies the target buffer object. The symbolic constant must be GL_ARRAY_BUFFER​, GL_ATOMIC_COUNTER_BUFFER​, GL_COPY_READ_BUFFER​, GL_COPY_WRITE_BUFFER​, GL_DRAW_INDIRECT_BUFFER​, GL_DISPATCH_INDIRECT_BUFFER​, GL_ELEMENT_ARRAY_BUFFER​, GL_PIXEL_PACK_BUFFER​, GL_PIXEL_UNPACK_BUFFER​, GL_QUERY_BUFFER​, GL_SHADER_STORAGE_BUFFER​, GL_TEXTURE_BUFFER​, GL_TRANSFORM_FEEDBACK_BUFFER​, or GL_UNIFORM_BUFFER​.</param>
        /// <param name="internalformat">The sized internal format with which the data will be stored in the buffer object.</param>
        /// <param name="format">Specifies the format of the pixel data. For transfers of depth, stencil, or depth/stencil data, you must use GL_DEPTH_COMPONENT​, GL_STENCIL_INDEX​, or GL_DEPTH_STENCIL​, where appropriate. For transfers of normalized integer or floating-point color image data, you must use one of the following: GL_RED​, GL_GREEN​, GL_BLUE​, GL_RG​, GL_RGB​, GL_BGR​, GL_RGBA​, and GL_BGRA​. For transfers of non-normalized integer data, you must use one of the following: GL_RED_INTEGER​, GL_GREEN_INTEGER​, GL_BLUE_INTEGER​, GL_RG_INTEGER​, GL_RGB_INTEGER​, GL_BGR_INTEGER​, GL_RGBA_INTEGER​, and GL_BGRA_INTEGER​.</param>
        /// <param name="type">Specifies the data type of the pixel data. The following symbolic values are accepted: GL_UNSIGNED_BYTE​, GL_BYTE​, GL_UNSIGNED_SHORT​, GL_SHORT​, GL_UNSIGNED_INT​, GL_INT​, GL_FLOAT​, GL_UNSIGNED_BYTE_3_3_2​, GL_UNSIGNED_BYTE_2_3_3_REV​, GL_UNSIGNED_SHORT_5_6_5​, GL_UNSIGNED_SHORT_5_6_5_REV​, GL_UNSIGNED_SHORT_4_4_4_4​, GL_UNSIGNED_SHORT_4_4_4_4_REV​, GL_UNSIGNED_SHORT_5_5_5_1​, GL_UNSIGNED_SHORT_1_5_5_5_REV​, GL_UNSIGNED_INT_8_8_8_8​, GL_UNSIGNED_INT_8_8_8_8_REV​, GL_UNSIGNED_INT_10_10_10_2​, and GL_UNSIGNED_INT_2_10_10_10_REV​.</param>
        /// <param name="data">Specifies a pointer to a single pixel of data to upload. This parameter may not be NULL.</param>
        public static void ClearBufferData(uint target, uint internalformat, uint format, uint type, IntPtr data)
        {
            GetDelegateFor<glClearBufferData>()(target, internalformat, format, type, data);
        }

        /// <summary>
        /// Fill all or part of buffer object's data store with a fixed value
        /// </summary>
        /// <param name="target">Specifies the target buffer object. The symbolic constant must be GL_ARRAY_BUFFER​, GL_ATOMIC_COUNTER_BUFFER​, GL_COPY_READ_BUFFER​, GL_COPY_WRITE_BUFFER​, GL_DRAW_INDIRECT_BUFFER​, GL_DISPATCH_INDIRECT_BUFFER​, GL_ELEMENT_ARRAY_BUFFER​, GL_PIXEL_PACK_BUFFER​, GL_PIXEL_UNPACK_BUFFER​, GL_QUERY_BUFFER​, GL_SHADER_STORAGE_BUFFER​, GL_TEXTURE_BUFFER​, GL_TRANSFORM_FEEDBACK_BUFFER​, or GL_UNIFORM_BUFFER​.</param>
        /// <param name="internalformat">The sized internal format with which the data will be stored in the buffer object.</param>
        /// <param name="offset">The offset, in basic machine units into the buffer object's data store at which to start filling.</param>
        /// <param name="size">The size, in basic machine units of the range of the data store to fill.</param>
        /// <param name="format">Specifies the format of the pixel data. For transfers of depth, stencil, or depth/stencil data, you must use GL_DEPTH_COMPONENT​, GL_STENCIL_INDEX​, or GL_DEPTH_STENCIL​, where appropriate. For transfers of normalized integer or floating-point color image data, you must use one of the following: GL_RED​, GL_GREEN​, GL_BLUE​, GL_RG​, GL_RGB​, GL_BGR​, GL_RGBA​, and GL_BGRA​. For transfers of non-normalized integer data, you must use one of the following: GL_RED_INTEGER​, GL_GREEN_INTEGER​, GL_BLUE_INTEGER​, GL_RG_INTEGER​, GL_RGB_INTEGER​, GL_BGR_INTEGER​, GL_RGBA_INTEGER​, and GL_BGRA_INTEGER​.</param>
        /// <param name="type">Specifies the data type of the pixel data. The following symbolic values are accepted: GL_UNSIGNED_BYTE​, GL_BYTE​, GL_UNSIGNED_SHORT​, GL_SHORT​, GL_UNSIGNED_INT​, GL_INT​, GL_FLOAT​, GL_UNSIGNED_BYTE_3_3_2​, GL_UNSIGNED_BYTE_2_3_3_REV​, GL_UNSIGNED_SHORT_5_6_5​, GL_UNSIGNED_SHORT_5_6_5_REV​, GL_UNSIGNED_SHORT_4_4_4_4​, GL_UNSIGNED_SHORT_4_4_4_4_REV​, GL_UNSIGNED_SHORT_5_5_5_1​, GL_UNSIGNED_SHORT_1_5_5_5_REV​, GL_UNSIGNED_INT_8_8_8_8​, GL_UNSIGNED_INT_8_8_8_8_REV​, GL_UNSIGNED_INT_10_10_10_2​, and GL_UNSIGNED_INT_2_10_10_10_REV​.</param>
        /// <param name="data">Specifies a pointer to a single pixel of data to upload. This parameter may not be NULL.</param>
        public static void ClearBufferSubData(uint target, uint internalformat, IntPtr offset, uint size, uint format, uint type, IntPtr data)
        {
            GetDelegateFor<glClearBufferSubData>()(target, internalformat, offset, size, format, type, data);
        }

        public static void ClearNamedBufferDataEXT(uint buffer, uint internalformat, uint format, uint type, IntPtr data)
        {
            GetDelegateFor<glClearNamedBufferDataEXT>()(buffer, internalformat, format, type, data);
        }
        public static void ClearNamedBufferSubDataEXT(uint buffer, uint internalformat, IntPtr offset, uint size, uint format, uint type, IntPtr data)
        {
            GetDelegateFor<glClearNamedBufferSubDataEXT>()(buffer, internalformat, offset, size, format, type, data);
        }

        //  Delegates
        private delegate void glClearBufferData(uint target, uint internalformat, uint format, uint type, IntPtr data);
        private delegate void glClearBufferSubData(uint target, uint internalformat, IntPtr offset, uint size, uint format, uint type, IntPtr data);
        private delegate void glClearNamedBufferDataEXT(uint buffer, uint internalformat, uint format, uint type, IntPtr data);
        private delegate void glClearNamedBufferSubDataEXT(uint buffer, uint internalformat, IntPtr offset, uint size, uint format, uint type, IntPtr data);

        #endregion

        #region GL_ARB_compute_shader

        /// <summary>
        /// Launch one or more compute work groups
        /// </summary>
        /// <param name="num_groups_x">The number of work groups to be launched in the X dimension.</param>
        /// <param name="num_groups_y">The number of work groups to be launched in the Y dimension.</param>
        /// <param name="num_groups_z">The number of work groups to be launched in the Z dimension.</param>
        public static void DispatchCompute(uint num_groups_x, uint num_groups_y, uint num_groups_z)
        {
            GetDelegateFor<glDispatchCompute>()(num_groups_x, num_groups_y, num_groups_z);
        }

        /// <summary>
        /// Launch one or more compute work groups using parameters stored in a buffer
        /// </summary>
        /// <param name="indirect">The offset into the buffer object currently bound to the GL_DISPATCH_INDIRECT_BUFFER​ buffer target at which the dispatch parameters are stored.</param>
        public static void DispatchComputeIndirect(IntPtr indirect)
        {
            GetDelegateFor<glDispatchComputeIndirect>()(indirect);
        }

        //  Delegates
        private delegate void glDispatchCompute(uint num_groups_x, uint num_groups_y, uint num_groups_z);
        private delegate void glDispatchComputeIndirect(IntPtr indirect);

        // Constants
        public const uint GL_COMPUTE_SHADER = 0x91B9;
        public const uint GL_MAX_COMPUTE_UNIFORM_BLOCKS = 0x91BB;
        public const uint GL_MAX_COMPUTE_TEXTURE_IMAGE_UNITS = 0x91BC;
        public const uint GL_MAX_COMPUTE_IMAGE_UNIFORMS = 0x91BD;
        public const uint GL_MAX_COMPUTE_SHARED_MEMORY_SIZE = 0x8262;
        public const uint GL_MAX_COMPUTE_UNIFORM_COMPONENTS = 0x8263;
        public const uint GL_MAX_COMPUTE_ATOMIC_COUNTER_BUFFERS = 0x8264;
        public const uint GL_MAX_COMPUTE_ATOMIC_COUNTERS = 0x8265;
        public const uint GL_MAX_COMBINED_COMPUTE_UNIFORM_COMPONENTS = 0x8266;
        public const uint GL_MAX_COMPUTE_WORK_GROUP_INVOCATIONS = 0x90EB;
        public const uint GL_MAX_COMPUTE_WORK_GROUP_COUNT = 0x91BE;
        public const uint GL_MAX_COMPUTE_WORK_GROUP_SIZE = 0x91BF;
        public const uint GL_COMPUTE_WORK_GROUP_SIZE = 0x8267;
        public const uint GL_UNIFORM_BLOCK_REFERENCED_BY_COMPUTE_SHADER = 0x90EC;
        public const uint GL_ATOMIC_COUNTER_BUFFER_REFERENCED_BY_COMPUTE_SHADER = 0x90ED;
        public const uint GL_DISPATCH_INDIRECT_BUFFER = 0x90EE;
        public const uint GL_DISPATCH_INDIRECT_BUFFER_BINDING = 0x90EF;
        public const uint GL_COMPUTE_SHADER_BIT = 0x00000020;

        #endregion

        #region GL_ARB_copy_image

        /// <summary>
        /// Perform a raw data copy between two images
        /// </summary>
        /// <param name="srcName">The name of a texture or renderbuffer object from which to copy.</param>
        /// <param name="srcTarget">The target representing the namespace of the source name srcName​.</param>
        /// <param name="srcLevel">The mipmap level to read from the source.</param>
        /// <param name="srcX">The X coordinate of the left edge of the souce region to copy.</param>
        /// <param name="srcY">The Y coordinate of the top edge of the souce region to copy.</param>
        /// <param name="srcZ">The Z coordinate of the near edge of the souce region to copy.</param>
        /// <param name="dstName">The name of a texture or renderbuffer object to which to copy.</param>
        /// <param name="dstTarget">The target representing the namespace of the destination name dstName​.</param>
        /// <param name="dstLevel">The desination mipmap level.</param>
        /// <param name="dstX">The X coordinate of the left edge of the destination region.</param>
        /// <param name="dstY">The Y coordinate of the top edge of the destination region.</param>
        /// <param name="dstZ">The Z coordinate of the near edge of the destination region.</param>
        /// <param name="srcWidth">The width of the region to be copied.</param>
        /// <param name="srcHeight">The height of the region to be copied.</param>
        /// <param name="srcDepth">The depth of the region to be copied.</param>
        public static void CopyImageSubData(uint srcName, uint srcTarget, int srcLevel, int srcX, int srcY, int srcZ, uint dstName,
           uint dstTarget, int dstLevel, int dstX, int dstY, int dstZ, uint srcWidth, uint srcHeight, uint srcDepth)
        {
            GetDelegateFor<glCopyImageSubData>()(srcName, srcTarget, srcLevel, srcX, srcY, srcZ, dstName,
            dstTarget, dstLevel, dstX, dstY, dstZ, srcWidth, srcHeight, srcDepth);
        }

        //  Delegates
        private delegate void glCopyImageSubData(uint srcName, uint srcTarget, int srcLevel, int srcX, int srcY, int srcZ, uint dstName,
            uint dstTarget, int dstLevel, int dstX, int dstY, int dstZ, uint srcWidth, uint srcHeight, uint srcDepth);

        #endregion

        #region GL_ARB_ES3_compatibility

        public const uint GL_COMPRESSED_RGB8_ETC2 = 0x9274;
        public const uint GL_COMPRESSED_SRGB8_ETC2 = 0x9275;
        public const uint GL_COMPRESSED_RGB8_PUNCHTHROUGH_ALPHA1_ETC2 = 0x9276;
        public const uint GL_COMPRESSED_SRGB8_PUNCHTHROUGH_ALPHA1_ETC2 = 0x9277;
        public const uint GL_COMPRESSED_RGBA8_ETC2_EAC = 0x9278;
        public const uint GL_COMPRESSED_SRGB8_ALPHA8_ETC2_EAC = 0x9279;
        public const uint GL_COMPRESSED_R11_EAC = 0x9270;
        public const uint GL_COMPRESSED_SIGNED_R11_EAC = 0x9271;
        public const uint GL_COMPRESSED_RG11_EAC = 0x9272;
        public const uint GL_COMPRESSED_SIGNED_RG11_EAC = 0x9273;
        public const uint GL_PRIMITIVE_RESTART_FIXED_INDEX = 0x8D69;
        public const uint GL_ANY_SAMPLES_PASSED_CONSERVATIVE = 0x8D6A;
        public const uint GL_MAX_ELEMENT_INDEX = 0x8D6B;
        public const uint GL_TEXTURE_IMMUTABLE_LEVELS = 0x82DF;

        #endregion

        #region GL_ARB_framebuffer_no_attachments

        //  Methods

        /// <summary>
        /// Set a named parameter of a framebuffer.
        /// </summary>
        /// <param name="target">The target of the operation, which must be GL_READ_FRAMEBUFFER​, GL_DRAW_FRAMEBUFFER​ or GL_FRAMEBUFFER​.</param>
        /// <param name="pname">A token indicating the parameter to be modified.</param>
        /// <param name="param">The new value for the parameter named pname​.</param>
        public static void FramebufferParameter(uint target, uint pname, int param)
        {
            GetDelegateFor<glFramebufferParameteri>()(target, pname, param);
        }

        /// <summary>
        /// Retrieve a named parameter from a framebuffer
        /// </summary>
        /// <param name="target">The target of the operation, which must be GL_READ_FRAMEBUFFER​, GL_DRAW_FRAMEBUFFER​ or GL_FRAMEBUFFER​.</param>
        /// <param name="pname">A token indicating the parameter to be retrieved.</param>
        /// <param name="parameters">The address of a variable to receive the value of the parameter named pname​.</param>
        public static void GetFramebufferParameter(uint target, uint pname, int[] parameters)
        {
            GetDelegateFor<glGetFramebufferParameteriv>()(target, pname, parameters);
        }

        public static void NamedFramebufferParameterEXT(uint framebuffer, uint pname, int param)
        {
            GetDelegateFor<glNamedFramebufferParameteriEXT>()(framebuffer, pname, param);
        }

        public static void GetNamedFramebufferParameterEXT(uint framebuffer, uint pname, int[] parameters)
        {
            GetDelegateFor<glGetNamedFramebufferParameterivEXT>()(framebuffer, pname, parameters);
        }

        //  Delegates
        private delegate void glFramebufferParameteri(uint target, uint pname, int param);
        private delegate void glGetFramebufferParameteriv(uint target, uint pname, int[] parameters);
        private delegate void glNamedFramebufferParameteriEXT(uint framebuffer, uint pname, int param);
        private delegate void glGetNamedFramebufferParameterivEXT(uint framebuffer, uint pname, int[] parameters);

        #endregion

        #region GL_ARB_internalformat_query2

        /// <summary>
        /// Retrieve information about implementation-dependent support for internal formats
        /// </summary>
        /// <param name="target">Indicates the usage of the internal format. target​ must be GL_TEXTURE_1D​, GL_TEXTURE_1D_ARRAY​, GL_TEXTURE_2D​, GL_TEXTURE_2D_ARRAY​, GL_TEXTURE_3D​, GL_TEXTURE_CUBE_MAP​, GL_TEXTURE_CUBE_MAP_ARRAY​, GL_TEXTURE_RECTANGLE​, GL_TEXTURE_BUFFER​, GL_RENDERBUFFER​, GL_TEXTURE_2D_MULTISAMPLE​ or GL_TEXTURE_2D_MULTISAMPLE_ARRAY​.</param>
        /// <param name="internalformat">Specifies the internal format about which to retrieve information.</param>
        /// <param name="pname">Specifies the type of information to query.</param>
        /// <param name="bufSize">Specifies the maximum number of basic machine units that may be written to params​ by the function.</param>
        /// <param name="parameters">Specifies the address of a variable into which to write the retrieved information.</param>
        public static void GetInternalformat(uint target, uint internalformat, uint pname, uint bufSize, int[] parameters)
        {
            GetDelegateFor<glGetInternalformativ>()(target, internalformat, pname, bufSize, parameters);
        }

        /// <summary>
        /// Retrieve information about implementation-dependent support for internal formats
        /// </summary>
        /// <param name="target">Indicates the usage of the internal format. target​ must be GL_TEXTURE_1D​, GL_TEXTURE_1D_ARRAY​, GL_TEXTURE_2D​, GL_TEXTURE_2D_ARRAY​, GL_TEXTURE_3D​, GL_TEXTURE_CUBE_MAP​, GL_TEXTURE_CUBE_MAP_ARRAY​, GL_TEXTURE_RECTANGLE​, GL_TEXTURE_BUFFER​, GL_RENDERBUFFER​, GL_TEXTURE_2D_MULTISAMPLE​ or GL_TEXTURE_2D_MULTISAMPLE_ARRAY​.</param>
        /// <param name="internalformat">Specifies the internal format about which to retrieve information.</param>
        /// <param name="pname">Specifies the type of information to query.</param>
        /// <param name="bufSize">Specifies the maximum number of basic machine units that may be written to params​ by the function.</param>
        /// <param name="parameters">Specifies the address of a variable into which to write the retrieved information.</param>
        public static void GetInternalformat(uint target, uint internalformat, uint pname, uint bufSize, Int64[] parameters)
        {
            GetDelegateFor<glGetInternalformati64v>()(target, internalformat, pname, bufSize, parameters);
        }

        //  Delegates
        private delegate void glGetInternalformativ(uint target, uint internalformat, uint pname, uint bufSize, int[] parameters);
        private delegate void glGetInternalformati64v(uint target, uint internalformat, uint pname, uint bufSize, Int64[] parameters);

        //  Constants
        public const uint GL_RENDERBUFFER = 0x8D41;
        public const uint GL_TEXTURE_2D_MULTISAMPLE = 0x9100;
        public const uint GL_TEXTURE_2D_MULTISAMPLE_ARRAY = 0x9102;
        public const uint GL_NUM_SAMPLE_COUNTS = 0x9380;
        public const uint GL_INTERNALFORMAT_SUPPORTED = 0x826F;
        public const uint GL_INTERNALFORMAT_PREFERRED = 0x8270;
        public const uint GL_INTERNALFORMAT_RED_SIZE = 0x8271;
        public const uint GL_INTERNALFORMAT_GREEN_SIZE = 0x8272;
        public const uint GL_INTERNALFORMAT_BLUE_SIZE = 0x8273;
        public const uint GL_INTERNALFORMAT_ALPHA_SIZE = 0x8274;
        public const uint GL_INTERNALFORMAT_DEPTH_SIZE = 0x8275;
        public const uint GL_INTERNALFORMAT_STENCIL_SIZE = 0x8276;
        public const uint GL_INTERNALFORMAT_SHARED_SIZE = 0x8277;
        public const uint GL_INTERNALFORMAT_RED_TYPE = 0x8278;
        public const uint GL_INTERNALFORMAT_GREEN_TYPE = 0x8279;
        public const uint GL_INTERNALFORMAT_BLUE_TYPE = 0x827A;
        public const uint GL_INTERNALFORMAT_ALPHA_TYPE = 0x827B;
        public const uint GL_INTERNALFORMAT_DEPTH_TYPE = 0x827C;
        public const uint GL_INTERNALFORMAT_STENCIL_TYPE = 0x827D;
        public const uint GL_MAX_WIDTH = 0x827E;
        public const uint GL_MAX_HEIGHT = 0x827F;
        public const uint GL_MAX_DEPTH = 0x8280;
        public const uint GL_MAX_LAYERS = 0x8281;
        public const uint GL_MAX_COMBINED_DIMENSIONS = 0x8282;
        public const uint GL_COLOR_COMPONENTS = 0x8283;
        public const uint GL_DEPTH_COMPONENTS = 0x8284;
        public const uint GL_STENCIL_COMPONENTS = 0x8285;
        public const uint GL_COLOR_RENDERABLE = 0x8286;
        public const uint GL_DEPTH_RENDERABLE = 0x8287;
        public const uint GL_STENCIL_RENDERABLE = 0x8288;
        public const uint GL_FRAMEBUFFER_RENDERABLE = 0x8289;
        public const uint GL_FRAMEBUFFER_RENDERABLE_LAYERED = 0x828A;
        public const uint GL_FRAMEBUFFER_BLEND = 0x828B;
        public const uint GL_READ_PIXELS = 0x828C;
        public const uint GL_READ_PIXELS_FORMAT = 0x828D;
        public const uint GL_READ_PIXELS_TYPE = 0x828E;
        public const uint GL_TEXTURE_IMAGE_FORMAT = 0x828F;
        public const uint GL_TEXTURE_IMAGE_TYPE = 0x8290;
        public const uint GL_GET_TEXTURE_IMAGE_FORMAT = 0x8291;
        public const uint GL_GET_TEXTURE_IMAGE_TYPE = 0x8292;
        public const uint GL_MIPMAP = 0x8293;
        public const uint GL_MANUAL_GENERATE_MIPMAP = 0x8294;
        public const uint GL_AUTO_GENERATE_MIPMAP = 0x8295;
        public const uint GL_COLOR_ENCODING = 0x8296;
        public const uint GL_SRGB_READ = 0x8297;
        public const uint GL_SRGB_WRITE = 0x8298;
        public const uint GL_SRGB_DECODE_ARB = 0x8299;
        public const uint GL_FILTER = 0x829A;
        public const uint GL_VERTEX_TEXTURE = 0x829B;
        public const uint GL_TESS_CONTROL_TEXTURE = 0x829C;
        public const uint GL_TESS_EVALUATION_TEXTURE = 0x829D;
        public const uint GL_GEOMETRY_TEXTURE = 0x829E;
        public const uint GL_FRAGMENT_TEXTURE = 0x829F;
        public const uint GL_COMPUTE_TEXTURE = 0x82A0;
        public const uint GL_TEXTURE_SHADOW = 0x82A1;
        public const uint GL_TEXTURE_GATHER = 0x82A2;
        public const uint GL_TEXTURE_GATHER_SHADOW = 0x82A3;
        public const uint GL_SHADER_IMAGE_LOAD = 0x82A4;
        public const uint GL_SHADER_IMAGE_STORE = 0x82A5;
        public const uint GL_SHADER_IMAGE_ATOMIC = 0x82A6;
        public const uint GL_IMAGE_TEXEL_SIZE = 0x82A7;
        public const uint GL_IMAGE_COMPATIBILITY_CLASS = 0x82A8;
        public const uint GL_IMAGE_PIXEL_FORMAT = 0x82A9;
        public const uint GL_IMAGE_PIXEL_TYPE = 0x82AA;
        public const uint GL_IMAGE_FORMAT_COMPATIBILITY_TYPE = 0x90C7;
        public const uint GL_SIMULTANEOUS_TEXTURE_AND_DEPTH_TEST = 0x82AC;
        public const uint GL_SIMULTANEOUS_TEXTURE_AND_STENCIL_TEST = 0x82AD;
        public const uint GL_SIMULTANEOUS_TEXTURE_AND_DEPTH_WRITE = 0x82AE;
        public const uint GL_SIMULTANEOUS_TEXTURE_AND_STENCIL_WRITE = 0x82AF;
        public const uint GL_TEXTURE_COMPRESSED_BLOCK_WIDTH = 0x82B1;
        public const uint GL_TEXTURE_COMPRESSED_BLOCK_HEIGHT = 0x82B2;
        public const uint GL_TEXTURE_COMPRESSED_BLOCK_SIZE = 0x82B3;
        public const uint GL_CLEAR_BUFFER = 0x82B4;
        public const uint GL_TEXTURE_VIEW = 0x82B5;
        public const uint GL_VIEW_COMPATIBILITY_CLASS = 0x82B6;
        public const uint GL_FULL_SUPPORT = 0x82B7;
        public const uint GL_CAVEAT_SUPPORT = 0x82B8;
        public const uint GL_IMAGE_CLASS_4_X_32 = 0x82B9;
        public const uint GL_IMAGE_CLASS_2_X_32 = 0x82BA;
        public const uint GL_IMAGE_CLASS_1_X_32 = 0x82BB;
        public const uint GL_IMAGE_CLASS_4_X_16 = 0x82BC;
        public const uint GL_IMAGE_CLASS_2_X_16 = 0x82BD;
        public const uint GL_IMAGE_CLASS_1_X_16 = 0x82BE;
        public const uint GL_IMAGE_CLASS_4_X_8 = 0x82BF;
        public const uint GL_IMAGE_CLASS_2_X_8 = 0x82C0;
        public const uint GL_IMAGE_CLASS_1_X_8 = 0x82C1;
        public const uint GL_IMAGE_CLASS_11_11_10 = 0x82C2;
        public const uint GL_IMAGE_CLASS_10_10_10_2 = 0x82C3;
        public const uint GL_VIEW_CLASS_128_BITS = 0x82C4;
        public const uint GL_VIEW_CLASS_96_BITS = 0x82C5;
        public const uint GL_VIEW_CLASS_64_BITS = 0x82C6;
        public const uint GL_VIEW_CLASS_48_BITS = 0x82C7;
        public const uint GL_VIEW_CLASS_32_BITS = 0x82C8;
        public const uint GL_VIEW_CLASS_24_BITS = 0x82C9;
        public const uint GL_VIEW_CLASS_16_BITS = 0x82CA;
        public const uint GL_VIEW_CLASS_8_BITS = 0x82CB;
        public const uint GL_VIEW_CLASS_S3TC_DXT1_RGB = 0x82CC;
        public const uint GL_VIEW_CLASS_S3TC_DXT1_RGBA = 0x82CD;
        public const uint GL_VIEW_CLASS_S3TC_DXT3_RGBA = 0x82CE;
        public const uint GL_VIEW_CLASS_S3TC_DXT5_RGBA = 0x82CF;
        public const uint GL_VIEW_CLASS_RGTC1_RED = 0x82D0;
        public const uint GL_VIEW_CLASS_RGTC2_RG = 0x82D1;
        public const uint GL_VIEW_CLASS_BPTC_UNORM = 0x82D2;
        public const uint GL_VIEW_CLASS_BPTC_FLOAT = 0x82D3;

        #endregion

        #region GL_ARB_invalidate_subdata

        /// <summary>
        /// Invalidate a region of a texture image
        /// </summary>
        /// <param name="texture">The name of a texture object a subregion of which to invalidate.</param>
        /// <param name="level">The level of detail of the texture object within which the region resides.</param>
        /// <param name="xoffset">The X offset of the region to be invalidated.</param>
        /// <param name="yoffset">The Y offset of the region to be invalidated.</param>
        /// <param name="zoffset">The Z offset of the region to be invalidated.</param>
        /// <param name="width">The width of the region to be invalidated.</param>
        /// <param name="height">The height of the region to be invalidated.</param>
        /// <param name="depth">The depth of the region to be invalidated.</param>
        public static void InvalidateTexSubImage(uint texture, int level, int xoffset, int yoffset, int zoffset,
           uint width, uint height, uint depth)
        {
            GetDelegateFor<glInvalidateTexSubImage>()(texture, level, xoffset, yoffset, zoffset, width, height, depth);
        }

        /// <summary>
        /// Invalidate the entirety a texture image
        /// </summary>
        /// <param name="texture">The name of a texture object to invalidate.</param>
        /// <param name="level">The level of detail of the texture object to invalidate.</param>
        public static void InvalidateTexImage(uint texture, int level)
        {
            GetDelegateFor<glInvalidateTexImage>()(texture, level);
        }

        /// <summary>
        /// Invalidate a region of a buffer object's data store
        /// </summary>
        /// <param name="buffer">The name of a buffer object, a subrange of whose data store to invalidate.</param>
        /// <param name="offset">The offset within the buffer's data store of the start of the range to be invalidated.</param>
        /// <param name="length">The length of the range within the buffer's data store to be invalidated.</param>
        public static void InvalidateBufferSubData(uint buffer, IntPtr offset, IntPtr length)
        {
            GetDelegateFor<glInvalidateBufferSubData>()(buffer, offset, length);
        }

        /// <summary>
        /// Invalidate the content of a buffer object's data store
        /// </summary>
        /// <param name="buffer">The name of a buffer object whose data store to invalidate.</param>
        public static void InvalidateBufferData(uint buffer)
        {
            GetDelegateFor<glInvalidateBufferData>()(buffer);
        }

        /// <summary>
        /// Invalidate the content some or all of a framebuffer object's attachments
        /// </summary>
        /// <param name="target">The target to which the framebuffer is attached. target​ must be GL_FRAMEBUFFER​, GL_DRAW_FRAMEBUFFER​, or GL_READ_FRAMEBUFFER​.</param>
        /// <param name="numAttachments">The number of entries in the attachments​ array.</param>
        /// <param name="attachments">The address of an array identifying the attachments to be invalidated.</param>
        public static void InvalidateFramebuffer(uint target, uint numAttachments, uint[] attachments)
        {
            GetDelegateFor<glInvalidateFramebuffer>()(target, numAttachments, attachments);
        }

        /// <summary>
        /// Invalidate the content of a region of some or all of a framebuffer object's attachments
        /// </summary>
        /// <param name="target">The target to which the framebuffer is attached. target​ must be GL_FRAMEBUFFER​, GL_DRAW_FRAMEBUFFER​, or GL_READ_FRAMEBUFFER​.</param>
        /// <param name="numAttachments">The number of entries in the attachments​ array.</param>
        /// <param name="attachments">The address of an array identifying the attachments to be invalidated.</param>
        /// <param name="x">The X offset of the region to be invalidated.</param>
        /// <param name="y">The Y offset of the region to be invalidated.</param>
        /// <param name="width">The width of the region to be invalidated.</param>
        /// <param name="height">The height of the region to be invalidated.</param>
        public static void InvalidateSubFramebuffer(uint target, uint numAttachments, uint[] attachments,
           int x, int y, uint width, uint height)
        {
            GetDelegateFor<glInvalidateSubFramebuffer>()(target, numAttachments, attachments, x, y, width, height);
        }

        //  Delegates
        private delegate void glInvalidateTexSubImage(uint texture, int level, int xoffset,
            int yoffset, int zoffset, uint width, uint height, uint depth);
        private delegate void glInvalidateTexImage(uint texture, int level);
        private delegate void glInvalidateBufferSubData(uint buffer, IntPtr offset, IntPtr length);
        private delegate void glInvalidateBufferData(uint buffer);
        private delegate void glInvalidateFramebuffer(uint target, uint numAttachments, uint[] attachments);
        private delegate void glInvalidateSubFramebuffer(uint target, uint numAttachments, uint[] attachments,
            int x, int y, uint width, uint height);

        #endregion

        #region ARB_multi_draw_indirect

        /// <summary>
        /// Render multiple sets of primitives from array data, taking parameters from memory
        /// </summary>
        /// <param name="mode">Specifies what kind of primitives to render. Symbolic constants GL_POINTS​, GL_LINE_STRIP​, GL_LINE_LOOP​, GL_LINES​, GL_LINE_STRIP_ADJACENCY​, GL_LINES_ADJACENCY​, GL_TRIANGLE_STRIP​, GL_TRIANGLE_FAN​, GL_TRIANGLES​, GL_TRIANGLE_STRIP_ADJACENCY​, GL_TRIANGLES_ADJACENCY​, and GL_PATCHES​ are accepted.</param>
        /// <param name="indirect">Specifies the address of an array of structures containing the draw parameters.</param>
        /// <param name="primcount">Specifies the the number of elements in the array of draw parameter structures.</param>
        /// <param name="stride">Specifies the distance in basic machine units between elements of the draw parameter array.</param>
        public static void MultiDrawArraysIndirect(uint mode, IntPtr indirect, uint primcount, uint stride)
        {
            GetDelegateFor<glMultiDrawArraysIndirect>()(mode, indirect, primcount, stride);
        }

        /// <summary>
        /// Render indexed primitives from array data, taking parameters from memory
        /// </summary>
        /// <param name="mode">Specifies what kind of primitives to render. Symbolic constants GL_POINTS​, GL_LINE_STRIP​, GL_LINE_LOOP​, GL_LINES​, GL_LINE_STRIP_ADJACENCY​, GL_LINES_ADJACENCY​, GL_TRIANGLE_STRIP​, GL_TRIANGLE_FAN​, GL_TRIANGLES​, GL_TRIANGLE_STRIP_ADJACENCY​, GL_TRIANGLES_ADJACENCY​, and GL_PATCHES​ are accepted.</param>
        /// <param name="type">Specifies the type of data in the buffer bound to the GL_ELEMENT_ARRAY_BUFFER​ binding.</param>
        /// <param name="indirect">Specifies a byte offset (cast to a pointer type) into the buffer bound to GL_DRAW_INDIRECT_BUFFER​, which designates the starting point of the structure containing the draw parameters.</param>
        /// <param name="primcount">Specifies the number of elements in the array addressed by indirect​.</param>
        /// <param name="stride">Specifies the distance in basic machine units between elements of the draw parameter array.</param>
        public static void MultiDrawElementsIndirect(uint mode, uint type, IntPtr indirect, uint primcount, uint stride)
        {
            GetDelegateFor<glMultiDrawElementsIndirect>()(mode, type, indirect, primcount, stride);
        }

        private delegate void glMultiDrawArraysIndirect(uint mode, IntPtr indirect, uint primcount, uint stride);
        private delegate void glMultiDrawElementsIndirect(uint mode, uint type, IntPtr indirect, uint primcount, uint stride);

        #endregion

        #region GL_ARB_program_interface_query

        /// <summary>
        /// Query a property of an interface in a program
        /// </summary>
        /// <param name="program">The name of a program object whose interface to query.</param>
        /// <param name="programInterface">A token identifying the interface within program​ to query.</param>
        /// <param name="pname">The name of the parameter within programInterface​ to query.</param>
        /// <param name="parameters">The address of a variable to retrieve the value of pname​ for the program interface..</param>
        public static void GetProgramInterface(uint program, uint programInterface, uint pname, int[] parameters)
        {
            GetDelegateFor<glGetProgramInterfaceiv>()(program, programInterface, pname, parameters);
        }

        /// <summary>
        /// Query the index of a named resource within a program
        /// </summary>
        /// <param name="program">The name of a program object whose resources to query.</param>
        /// <param name="programInterface">A token identifying the interface within program​ containing the resource named name​.</param>
        /// <param name="name">The name of the resource to query the index of.</param>
        public static void GetProgramResourceIndex(uint program, uint programInterface, string name)
        {
            GetDelegateFor<glGetProgramResourceIndex>()(program, programInterface, name);
        }

        /// <summary>
        /// Query the name of an indexed resource within a program
        /// </summary>
        /// <param name="program">The name of a program object whose resources to query.</param>
        /// <param name="programInterface">A token identifying the interface within program​ containing the indexed resource.</param>
        /// <param name="index">The index of the resource within programInterface​ of program​.</param>
        /// <param name="bufSize">The size of the character array whose address is given by name​.</param>
        /// <param name="length">The address of a variable which will receive the length of the resource name.</param>
        /// <param name="name">The address of a character array into which will be written the name of the resource.</param>
        public static void GetProgramResourceName(uint program, uint programInterface, uint index, uint bufSize, out uint length, out string name)
        {
            var lengthParameter = new uint[1];
            var nameParameter = new string[1];
            GetDelegateFor<glGetProgramResourceName>()(program, programInterface, index, bufSize, lengthParameter, nameParameter);
            length = lengthParameter[0];
            name = nameParameter[0];
        }

        /// <summary>
        /// Retrieve values for multiple properties of a single active resource within a program object
        /// </summary>
        /// <param name="program">The name of a program object whose resources to query.</param>
        /// <param name="programInterface">A token identifying the interface within program​ containing the resource named name​.</param>
        /// <param name="index">The index within the programInterface​ to query information about.</param>
        /// <param name="propCount">The number of properties being queried.</param>
        /// <param name="props">An array of properties of length propCount​ to query.</param>
        /// <param name="bufSize">The number of GLint values in the params​ array.</param>
        /// <param name="length">If not NULL, then this value will be filled in with the number of actual parameters written to params​.</param>
        /// <param name="parameters">The output array of parameters to write.</param>
        public static void GetProgramResource(uint program, uint programInterface, uint index, uint propCount, uint[] props, uint bufSize, out uint length, out int[] parameters)
        {
            var lengthParameter = new uint[1];
            var parametersParameter = new int[bufSize];

            GetDelegateFor<glGetProgramResourceiv>()(program, programInterface, index, propCount, props, bufSize, lengthParameter, parametersParameter);
            length = lengthParameter[0];
            parameters = parametersParameter;
        }

        /// <summary>
        /// Query the location of a named resource within a program.
        /// </summary>
        /// <param name="program">The name of a program object whose resources to query.</param>
        /// <param name="programInterface">A token identifying the interface within program​ containing the resource named name​.</param>
        /// <param name="name">The name of the resource to query the location of.</param>
        public static void GetProgramResourceLocation(uint program, uint programInterface, string name)
        {
            GetDelegateFor<glGetProgramResourceLocation>()(program, programInterface, name);
        }

        /// <summary>
        /// Query the fragment color index of a named variable within a program.
        /// </summary>
        /// <param name="program">The name of a program object whose resources to query.</param>
        /// <param name="programInterface">A token identifying the interface within program​ containing the resource named name​.</param>
        /// <param name="name">The name of the resource to query the location of.</param>
        public static void GetProgramResourceLocationIndex(uint program, uint programInterface, string name)
        {
            GetDelegateFor<glGetProgramResourceLocationIndex>()(program, programInterface, name);
        }

        private delegate void glGetProgramInterfaceiv(uint program, uint programInterface, uint pname, int[] parameters);
        private delegate uint glGetProgramResourceIndex(uint program, uint programInterface, string name);
        private delegate void glGetProgramResourceName(uint program, uint programInterface, uint index, uint bufSize, uint[] length, string[] name);
        private delegate void glGetProgramResourceiv(uint program, uint programInterface, uint index, uint propCount, uint[] props, uint bufSize, uint[] length, int[] parameters);
        private delegate int glGetProgramResourceLocation(uint program, uint programInterface, string name);
        private delegate int glGetProgramResourceLocationIndex(uint program, uint programInterface, string name);

        #endregion

        #region GL_ARB_shader_storage_buffer_object

        /// <summary>
        /// Change an active shader storage block binding.
        /// </summary>
        /// <param name="program">The name of the program containing the block whose binding to change.</param>
        /// <param name="storageBlockIndex">The index storage block within the program.</param>
        /// <param name="storageBlockBinding">The index storage block binding to associate with the specified storage block.</param>
        public static void ShaderStorageBlockBinding(uint program, uint storageBlockIndex, uint storageBlockBinding)
        {
            GetDelegateFor<glShaderStorageBlockBinding>()(program, storageBlockIndex, storageBlockBinding);
        }

        private delegate void glShaderStorageBlockBinding(uint program, uint storageBlockIndex, uint storageBlockBinding);

        //  Constants
        public const uint GL_SHADER_STORAGE_BUFFER = 0x90D2;
        public const uint GL_SHADER_STORAGE_BUFFER_BINDING = 0x90D3;
        public const uint GL_SHADER_STORAGE_BUFFER_START = 0x90D4;
        public const uint GL_SHADER_STORAGE_BUFFER_SIZE = 0x90D5;
        public const uint GL_MAX_VERTEX_SHADER_STORAGE_BLOCKS = 0x90D6;
        public const uint GL_MAX_GEOMETRY_SHADER_STORAGE_BLOCKS = 0x90D7;
        public const uint GL_MAX_TESS_CONTROL_SHADER_STORAGE_BLOCKS = 0x90D8;
        public const uint GL_MAX_TESS_EVALUATION_SHADER_STORAGE_BLOCKS = 0x90D9;
        public const uint GL_MAX_FRAGMENT_SHADER_STORAGE_BLOCKS = 0x90DA;
        public const uint GL_MAX_COMPUTE_SHADER_STORAGE_BLOCKS = 0x90DB;
        public const uint GL_MAX_COMBINED_SHADER_STORAGE_BLOCKS = 0x90DC;
        public const uint GL_MAX_SHADER_STORAGE_BUFFER_BINDINGS = 0x90DD;
        public const uint GL_MAX_SHADER_STORAGE_BLOCK_SIZE = 0x90DE;
        public const uint GL_SHADER_STORAGE_BUFFER_OFFSET_ALIGNMENT = 0x90DF;
        public const uint GL_SHADER_STORAGE_BARRIER_BIT = 0x2000;
        public const uint GL_MAX_COMBINED_SHADER_OUTPUT_RESOURCES = 0x8F39;

        #endregion

        #region GL_ARB_stencil_texturing

        //  Constants
        public const uint GL_DEPTH_STENCIL_TEXTURE_MODE = 0x90EA;

        #endregion

        #region GL_ARB_texture_buffer_range

        /// <summary>
        /// Bind a range of a buffer's data store to a buffer texture
        /// </summary>
        /// <param name="target">Specifies the target of the operation and must be GL_TEXTURE_BUFFER​.</param>
        /// <param name="internalformat">Specifies the internal format of the data in the store belonging to buffer​.</param>
        /// <param name="buffer">Specifies the name of the buffer object whose storage to attach to the active buffer texture.</param>
        /// <param name="offset">Specifies the offset of the start of the range of the buffer's data store to attach.</param>
        /// <param name="size">Specifies the size of the range of the buffer's data store to attach.</param>
        public static void TexBufferRange(uint target, uint internalformat, uint buffer, IntPtr offset, IntPtr size)
        {
            GetDelegateFor<glTexBufferRange>()(target, internalformat, buffer, offset, size);
        }

        /// <summary>
        /// Bind a range of a buffer's data store to a buffer texture
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="target">Specifies the target of the operation and must be GL_TEXTURE_BUFFER​.</param>
        /// <param name="internalformat">Specifies the internal format of the data in the store belonging to buffer​.</param>
        /// <param name="buffer">Specifies the name of the buffer object whose storage to attach to the active buffer texture.</param>
        /// <param name="offset">Specifies the offset of the start of the range of the buffer's data store to attach.</param>
        /// <param name="size">Specifies the size of the range of the buffer's data store to attach.</param>
        public static void TextureBufferRangeEXT(uint texture, uint target, uint internalformat, uint buffer, IntPtr offset, IntPtr size)
        {
            GetDelegateFor<glTextureBufferRangeEXT>()(texture, target, internalformat, buffer, offset, size);
        }

        private delegate void glTexBufferRange(uint target, uint internalformat, uint buffer, IntPtr offset, IntPtr size);
        private delegate void glTextureBufferRangeEXT(uint texture, uint target, uint internalformat, uint buffer, IntPtr offset, IntPtr size);

        #endregion

        #region GL_ARB_texture_storage_multisample

        /// <summary>
        /// Specify storage for a two-dimensional multisample texture.
        /// </summary>
        /// <param name="target">Specify the target of the operation. target​ must be GL_TEXTURE_2D_MULTISAMPLE​ or GL_PROXY_TEXTURE_2D_MULTISAMPLE​.</param>
        /// <param name="samples">Specify the number of samples in the texture.</param>
        /// <param name="internalformat">Specifies the sized internal format to be used to store texture image data.</param>
        /// <param name="width">Specifies the width of the texture, in texels.</param>
        /// <param name="height">Specifies the height of the texture, in texels.</param>
        /// <param name="fixedsamplelocations">Specifies whether the image will use identical sample locations and the same number of samples for all texels in the image, and the sample locations will not depend on the internal format or size of the image.</param>
        public static void TexStorage2DMultisample(uint target, uint samples, uint internalformat, uint width, uint height, bool fixedsamplelocations)
        {
            GetDelegateFor<glTexStorage2DMultisample>()(target, samples, internalformat, width, height, fixedsamplelocations);
        }

        /// <summary>
        /// Specify storage for a three-dimensional multisample array texture
        /// </summary>
        /// <param name="target">Specify the target of the operation. target​ must be GL_TEXTURE_3D_MULTISAMPLE_ARRAY​ or GL_PROXY_TEXTURE_3D_MULTISAMPLE_ARRAY​.</param>
        /// <param name="samples">Specify the number of samples in the texture.</param>
        /// <param name="internalformat">Specifies the sized internal format to be used to store texture image data.</param>
        /// <param name="width">Specifies the width of the texture, in texels.</param>
        /// <param name="height">Specifies the height of the texture, in texels.</param>
        /// <param name="depth">Specifies the depth of the texture, in layers.</param>
        /// <param name="fixedsamplelocations">Specifies the depth of the texture, in layers.</param>
        public static void TexStorage3DMultisample(uint target, uint samples, uint internalformat, uint width, uint height, uint depth, bool fixedsamplelocations)
        {
            GetDelegateFor<glTexStorage3DMultisample>()(target, samples, internalformat, width, height, depth, fixedsamplelocations);
        }

        /// <summary>
        /// Specify storage for a two-dimensional multisample texture.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="target">Specify the target of the operation. target​ must be GL_TEXTURE_2D_MULTISAMPLE​ or GL_PROXY_TEXTURE_2D_MULTISAMPLE​.</param>
        /// <param name="samples">Specify the number of samples in the texture.</param>
        /// <param name="internalformat">Specifies the sized internal format to be used to store texture image data.</param>
        /// <param name="width">Specifies the width of the texture, in texels.</param>
        /// <param name="height">Specifies the height of the texture, in texels.</param>
        /// <param name="fixedsamplelocations">Specifies whether the image will use identical sample locations and the same number of samples for all texels in the image, and the sample locations will not depend on the internal format or size of the image.</param>
        public static void TexStorage2DMultisampleEXT(uint texture, uint target, uint samples, uint internalformat, uint width, uint height, bool fixedsamplelocations)
        {
            GetDelegateFor<glTexStorage2DMultisampleEXT>()(texture, target, samples, internalformat, width, height, fixedsamplelocations);
        }

        /// <summary>
        /// Specify storage for a three-dimensional multisample array texture
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="target">Specify the target of the operation. target​ must be GL_TEXTURE_3D_MULTISAMPLE_ARRAY​ or GL_PROXY_TEXTURE_3D_MULTISAMPLE_ARRAY​.</param>
        /// <param name="samples">Specify the number of samples in the texture.</param>
        /// <param name="internalformat">Specifies the sized internal format to be used to store texture image data.</param>
        /// <param name="width">Specifies the width of the texture, in texels.</param>
        /// <param name="height">Specifies the height of the texture, in texels.</param>
        /// <param name="depth">Specifies the depth of the texture, in layers.</param>
        /// <param name="fixedsamplelocations">Specifies the depth of the texture, in layers.</param>
        public static void TexStorage3DMultisampleEXT(uint texture, uint target, uint samples, uint internalformat, uint width, uint height, uint depth, bool fixedsamplelocations)
        {
            GetDelegateFor<glTexStorage3DMultisampleEXT>()(texture, target, samples, internalformat, width, height, depth, fixedsamplelocations);
        }

        //  Delegates
        private delegate void glTexStorage2DMultisample(uint target, uint samples, uint internalformat, uint width, uint height, bool fixedsamplelocations);
        private delegate void glTexStorage3DMultisample(uint target, uint samples, uint internalformat, uint width, uint height, uint depth, bool fixedsamplelocations);
        private delegate void glTexStorage2DMultisampleEXT(uint texture, uint target, uint samples, uint internalformat, uint width, uint height, bool fixedsamplelocations);
        private delegate void glTexStorage3DMultisampleEXT(uint texture, uint target, uint samples, uint internalformat, uint width, uint height, uint depth, bool fixedsamplelocations);

        #endregion

        #region GL_ARB_texture_view

        /// <summary>
        /// Initialize a texture as a data alias of another texture's data store.
        /// </summary>
        /// <param name="texture">Specifies the texture object to be initialized as a view.</param>
        /// <param name="target">Specifies the target to be used for the newly initialized texture.</param>
        /// <param name="origtexture">Specifies the name of a texture object of which to make a view.</param>
        /// <param name="internalformat">Specifies the internal format for the newly created view.</param>
        /// <param name="minlevel">Specifies lowest level of detail of the view.</param>
        /// <param name="numlevels">Specifies the number of levels of detail to include in the view.</param>
        /// <param name="minlayer">Specifies the index of the first layer to include in the view.</param>
        /// <param name="numlayers">Specifies the number of layers to include in the view.</param>
        public static void TextureView(uint texture, uint target, uint origtexture, uint internalformat, uint minlevel, uint numlevels, uint minlayer, uint numlayers)
        {
            GetDelegateFor<glTextureView>()(texture, target, origtexture, internalformat, minlevel, numlevels, minlayer, numlayers);
        }

        //  Delegates
        private delegate void glTextureView(uint texture, uint target, uint origtexture, uint internalformat, uint minlevel, uint numlevels, uint minlayer, uint numlayers);

        //  Constants
        public const uint GL_TEXTURE_VIEW_MIN_LEVEL = 0x82DB;
        public const uint GL_TEXTURE_VIEW_NUM_LEVELS = 0x82DC;
        public const uint GL_TEXTURE_VIEW_MIN_LAYER = 0x82DD;
        public const uint GL_TEXTURE_VIEW_NUM_LAYERS = 0x82DE;

        #endregion

        #region GL_ARB_vertex_attrib_binding

        /// <summary>
        /// Bind a buffer to a vertex buffer bind point.
        /// </summary>
        /// <param name="bindingindex">The index of the vertex buffer binding point to which to bind the buffer.</param>
        /// <param name="buffer">The name of an existing buffer to bind to the vertex buffer binding point.</param>
        /// <param name="offset">The offset of the first element of the buffer.</param>
        /// <param name="stride">The distance between elements within the buffer.</param>
        public static void BindVertexBuffer(uint bindingindex, uint buffer, IntPtr offset, uint stride)
        {
            GetDelegateFor<glBindVertexBuffer>()(bindingindex, buffer, offset, stride);
        }

        /// <summary>
        /// Specify the organization of vertex arrays.
        /// </summary>
        /// <param name="attribindex">The generic vertex attribute array being described.</param>
        /// <param name="size">The number of values per vertex that are stored in the array.</param>
        /// <param name="type">The type of the data stored in the array.</param>
        /// <param name="normalized">GL_TRUE​ if the parameter represents a normalized integer (type​ must be an integer type). GL_FALSE​ otherwise.</param>
        /// <param name="relativeoffset">The offset, measured in basic machine units of the first element relative to the start of the vertex buffer binding this attribute fetches from.</param>
        public static void VertexAttribFormat(uint attribindex, int size, uint type, bool normalized, uint relativeoffset)
        {
            GetDelegateFor<glVertexAttribFormat>()(attribindex, size, type, normalized, relativeoffset);
        }

        /// <summary>
        /// Specify the organization of vertex arrays.
        /// </summary>
        /// <param name="attribindex">The generic vertex attribute array being described.</param>
        /// <param name="size">The number of values per vertex that are stored in the array.</param>
        /// <param name="type">The type of the data stored in the array.</param>
        /// <param name="relativeoffset">The offset, measured in basic machine units of the first element relative to the start of the vertex buffer binding this attribute fetches from.</param>
        public static void VertexAttribIFormat(uint attribindex, int size, uint type, uint relativeoffset)
        {
            GetDelegateFor<glVertexAttribIFormat>()(attribindex, size, type, relativeoffset);
        }

        /// <summary>
        /// Specify the organization of vertex arrays.
        /// </summary>
        /// <param name="attribindex">The generic vertex attribute array being described.</param>
        /// <param name="size">The number of values per vertex that are stored in the array.</param>
        /// <param name="type">The type of the data stored in the array.</param>
        /// <param name="relativeoffset">The offset, measured in basic machine units of the first element relative to the start of the vertex buffer binding this attribute fetches from.</param>
        public static void VertexAttribLFormat(uint attribindex, int size, uint type, uint relativeoffset)
        {
            GetDelegateFor<glVertexAttribLFormat>()(attribindex, size, type, relativeoffset);
        }

        /// <summary>
        /// Associate a vertex attribute and a vertex buffer binding.
        /// </summary>
        /// <param name="attribindex">The index of the attribute to associate with a vertex buffer binding.</param>
        /// <param name="bindingindex">The index of the vertex buffer binding with which to associate the generic vertex attribute.</param>
        public static void VertexAttribBinding(uint attribindex, uint bindingindex)
        {
            GetDelegateFor<glVertexAttribBinding>()(attribindex, bindingindex);
        }

        /// <summary>
        /// Modify the rate at which generic vertex attributes advance.
        /// </summary>
        /// <param name="bindingindex">The index of the binding whose divisor to modify.</param>
        /// <param name="divisor">The new value for the instance step rate to apply.</param>
        public static void VertexBindingDivisor(uint bindingindex, uint divisor)
        {
            GetDelegateFor<glVertexBindingDivisor>()(bindingindex, divisor);
        }

        /// <summary>
        /// Bind a buffer to a vertex buffer bind point.
        /// Available only when When EXT_direct_state_access is present.
        /// </summary>
        /// <param name="vaobj">The vertex array object.</param>
        /// <param name="bindingindex">The index of the vertex buffer binding point to which to bind the buffer.</param>
        /// <param name="buffer">The name of an existing buffer to bind to the vertex buffer binding point.</param>
        /// <param name="offset">The offset of the first element of the buffer.</param>
        /// <param name="stride">The distance between elements within the buffer.</param>
        public static void VertexArrayBindVertexBufferEXT(uint vaobj, uint bindingindex, uint buffer, IntPtr offset, uint stride)
        {
            GetDelegateFor<glVertexArrayBindVertexBufferEXT>()(vaobj, bindingindex, buffer, offset, stride);
        }

        /// <summary>
        /// Specify the organization of vertex arrays.
        /// Available only when When EXT_direct_state_access is present.
        /// </summary>
        /// <param name="vaobj">The vertex array object.</param>
        /// <param name="attribindex">The generic vertex attribute array being described.</param>
        /// <param name="size">The number of values per vertex that are stored in the array.</param>
        /// <param name="type">The type of the data stored in the array.</param>
        /// <param name="normalized">GL_TRUE​ if the parameter represents a normalized integer (type​ must be an integer type). GL_FALSE​ otherwise.</param>
        /// <param name="relativeoffset">The offset, measured in basic machine units of the first element relative to the start of the vertex buffer binding this attribute fetches from.</param>
        public static void VertexArrayVertexAttribFormatEXT(uint vaobj, uint attribindex, int size, uint type, bool normalized, uint relativeoffset)
        {
            GetDelegateFor<glVertexArrayVertexAttribFormatEXT>()(vaobj, attribindex, size, type, normalized, relativeoffset);
        }

        /// <summary>
        /// Specify the organization of vertex arrays.
        /// Available only when When EXT_direct_state_access is present.
        /// </summary>
        /// <param name="vaobj">The vertex array object.</param>
        /// <param name="attribindex">The generic vertex attribute array being described.</param>
        /// <param name="size">The number of values per vertex that are stored in the array.</param>
        /// <param name="type">The type of the data stored in the array.</param>
        /// <param name="relativeoffset">The offset, measured in basic machine units of the first element relative to the start of the vertex buffer binding this attribute fetches from.</param>
        public static void VertexArrayVertexAttribIFormatEXT(uint vaobj, uint attribindex, int size, uint type, uint relativeoffset)
        {
            GetDelegateFor<glVertexArrayVertexAttribIFormatEXT>()(vaobj, attribindex, size, type, relativeoffset);
        }

        /// <summary>
        /// Specify the organization of vertex arrays.
        /// Available only when When EXT_direct_state_access is present.
        /// </summary>
        /// <param name="vaobj">The vertex array object.</param>
        /// <param name="attribindex">The generic vertex attribute array being described.</param>
        /// <param name="size">The number of values per vertex that are stored in the array.</param>
        /// <param name="type">The type of the data stored in the array.</param>
        /// <param name="relativeoffset">The offset, measured in basic machine units of the first element relative to the start of the vertex buffer binding this attribute fetches from.</param>
        public static void VertexArrayVertexAttribLFormatEXT(uint vaobj, uint attribindex, int size, uint type, uint relativeoffset)
        {
            GetDelegateFor<glVertexArrayVertexAttribLFormatEXT>()(vaobj, attribindex, size, type, relativeoffset);
        }

        /// <summary>
        /// Associate a vertex attribute and a vertex buffer binding.
        /// Available only when When EXT_direct_state_access is present.
        /// </summary>
        /// <param name="vaobj">The vertex array object.</param>
        /// <param name="attribindex">The index of the attribute to associate with a vertex buffer binding.</param>
        /// <param name="bindingindex">The index of the vertex buffer binding with which to associate the generic vertex attribute.</param>
        public static void VertexArrayVertexAttribBindingEXT(uint vaobj, uint attribindex, uint bindingindex)
        {
            GetDelegateFor<glVertexArrayVertexAttribBindingEXT>()(vaobj, attribindex, bindingindex);
        }

        /// <summary>
        /// Modify the rate at which generic vertex attributes advance.
        /// Available only when When EXT_direct_state_access is present.
        /// </summary>
        /// <param name="vaobj">The vertex array object.</param>
        /// <param name="bindingindex">The index of the binding whose divisor to modify.</param>
        /// <param name="divisor">The new value for the instance step rate to apply.</param>
        public static void VertexArrayVertexBindingDivisorEXT(uint vaobj, uint bindingindex, uint divisor)
        {
            GetDelegateFor<glVertexArrayVertexBindingDivisorEXT>()(vaobj, bindingindex, divisor);
        }

        //  Delegates
        private delegate void glBindVertexBuffer(uint bindingindex, uint buffer, IntPtr offset, uint stride);
        private delegate void glVertexAttribFormat(uint attribindex, int size, uint type, bool normalized, uint relativeoffset);
        private delegate void glVertexAttribIFormat(uint attribindex, int size, uint type, uint relativeoffset);
        private delegate void glVertexAttribLFormat(uint attribindex, int size, uint type, uint relativeoffset);
        private delegate void glVertexAttribBinding(uint attribindex, uint bindingindex);
        private delegate void glVertexBindingDivisor(uint bindingindex, uint divisor);
        private delegate void glVertexArrayBindVertexBufferEXT(uint vaobj, uint bindingindex, uint buffer, IntPtr offset, uint stride);
        private delegate void glVertexArrayVertexAttribFormatEXT(uint vaobj, uint attribindex, int size, uint type, bool normalized, uint relativeoffset);
        private delegate void glVertexArrayVertexAttribIFormatEXT(uint vaobj, uint attribindex, int size, uint type, uint relativeoffset);
        private delegate void glVertexArrayVertexAttribLFormatEXT(uint vaobj, uint attribindex, int size, uint type, uint relativeoffset);
        private delegate void glVertexArrayVertexAttribBindingEXT(uint vaobj, uint attribindex, uint bindingindex);
        private delegate void glVertexArrayVertexBindingDivisorEXT(uint vaobj, uint bindingindex, uint divisor);

        //  Constants
        public const uint GL_VERTEX_ATTRIB_BINDING = 0x82D4;
        public const uint GL_VERTEX_ATTRIB_RELATIVE_OFFSET = 0x82D5;
        public const uint GL_VERTEX_BINDING_DIVISOR = 0x82D6;
        public const uint GL_VERTEX_BINDING_OFFSET = 0x82D7;
        public const uint GL_VERTEX_BINDING_STRIDE = 0x82D8;
        public const uint GL_VERTEX_BINDING_BUFFER = 0x8F4F;
        public const uint GL_MAX_VERTEX_ATTRIB_RELATIVE_OFFSET = 0x82D9;
        public const uint GL_MAX_VERTEX_ATTRIB_BINDINGS = 0x82DA;

        #endregion

        #endregion
    }
}
