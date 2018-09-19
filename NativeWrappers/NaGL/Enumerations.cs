using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL
{
    /// <summary>
    ///  AccumOp
    /// </summary>
    public enum AccumOperation : uint
    {
        Accum = GL.GL_ACCUM,
        Load = GL.GL_LOAD,
        Return = GL.GL_RETURN,
        Multiple = GL.GL_MULT,
        Add = GL.GL_ADD
    }

    /// <summary>
    /// The alpha function
    /// </summary>
    public enum AlphaTestFunction : uint
    {
        Never = GL.GL_NEVER,
        Less = GL.GL_LESS,
        Equal = GL.GL_EQUAL,
        LessThanOrEqual = GL.GL_LEQUAL,
        Great = GL.GL_GREATER,
        NotEqual = GL.GL_NOTEQUAL,
        GreaterThanOrEqual = GL.GL_GEQUAL,
        Always = GL.GL_ALWAYS,
    }
    
    /// <summary>
    /// The OpenGL Attribute flags.
    /// </summary>
    [Flags]
    public enum AttributeMask : uint
    {
        None = 0,
        Current = GL.GL_CURRENT_BIT,
        Point = GL.GL_POINT_BIT,
        Line = GL.GL_LINE_BIT,
        Polygon = GL.GL_POLYGON_BIT,
        PolygonStipple = GL.GL_POLYGON_STIPPLE_BIT,
        PixelMode = GL.GL_PIXEL_MODE_BIT,
        Lighting = GL.GL_LIGHTING_BIT,
        Fog = GL.GL_FOG_BIT,
        DepthBuffer = GL.GL_DEPTH_BUFFER_BIT,
        AccumBuffer = GL.GL_ACCUM_BUFFER_BIT,
        StencilBuffer = GL.GL_STENCIL_BUFFER_BIT,
        Viewport = GL.GL_VIEWPORT_BIT,
        Transform = GL.GL_TRANSFORM_BIT,
        Enable = GL.GL_ENABLE_BIT,
        ColorBuffer = GL.GL_COLOR_BUFFER_BIT,
        Hint = GL.GL_HINT_BIT,
        Eval = GL.GL_EVAL_BIT,
        List = GL.GL_LIST_BIT,
        Texture = GL.GL_TEXTURE_BIT,
        Scissor = GL.GL_SCISSOR_BIT,
        All = GL.GL_ALL_ATTRIB_BITS,
    }

    /// <summary>
    /// The begin mode.
    /// </summary>
    public enum PrimitiveType : uint
    {
        Points = GL.GL_POINTS,
        Lines = GL.GL_LINES,
        LineLoop = GL.GL_LINE_LOOP,
        LineStrip = GL.GL_LINE_STRIP,
        Triangles = GL.GL_TRIANGLES,
        TriangleString = GL.GL_TRIANGLE_STRIP,
        TriangleFan = GL.GL_TRIANGLE_FAN,
        Quads= GL.GL_QUADS,
        QuadStrip = GL.GL_QUAD_STRIP,
        Polygon = GL.GL_POLYGON
    }
    
    /// <summary>
    /// BlendingDestinationFactor
    /// </summary>
    public enum BlendingDestinationFactor : uint
    {
        Zero = GL.GL_ZERO,
        One = GL.GL_ONE,
        SourceColor = GL.GL_SRC_COLOR,
        OneMinusSourceColor = GL.GL_ONE_MINUS_SRC_COLOR,
        SourceAlpha = GL.GL_SRC_ALPHA,
        OneMinusSourceAlpha = GL.GL_ONE_MINUS_SRC_ALPHA,
        DestinationAlpha = GL.GL_DST_ALPHA,
        OneMinusDestinationAlpha = GL.GL_ONE_MINUS_DST_ALPHA,
    }

    /// <summary>
    /// The blending source factor.
    /// </summary>
    public enum BlendingSourceFactor : uint
    {
        DestinationColor = GL.GL_DST_COLOR,
        OneMinusDestinationColor = GL.GL_ONE_MINUS_DST_COLOR,
        SourceAlphaSaturate = GL.GL_SRC_ALPHA_SATURATE,
        /// <summary>
        /// 
        /// </summary>
        SourceAlpha = GL.GL_SRC_ALPHA,
    }

    public enum BufferTarget : uint
    {
        Array = GL.GL_ARRAY_BUFFER,
        ElementArray = GL.GL_ELEMENT_ARRAY_BUFFER,
    }

    public enum ClearBufferMask : uint
    {
        ColorBufferBit = GL.GL_COLOR_BUFFER_BIT,
        DepthBufferBit = GL.GL_DEPTH_BUFFER_BIT,
        StencilBufferBit = GL.GL_STENCIL_BUFFER_BIT,
    }
    
    /// <summary>
    /// The Clip Plane Name
    /// </summary>
    public enum ClipPlaneName : uint
    {
        ClipPlane0 = GL.GL_CLIP_PLANE0,
        ClipPlane1 = GL.GL_CLIP_PLANE1,
        ClipPlane2 = GL.GL_CLIP_PLANE2,
        ClipPlane3 = GL.GL_CLIP_PLANE3,
        ClipPlane4 = GL.GL_CLIP_PLANE4,
        ClipPlane5 = GL.GL_CLIP_PLANE5
    }

    /// <summary>
    /// The Cull Face mode.
    /// </summary>
    public enum FaceMode : uint
    {
        /// <summary>
        /// 
        /// </summary>
        Front = GL.GL_FRONT,
        FrontAndBack = GL.GL_FRONT_AND_BACK,
        Back = GL.GL_BACK,
    }

    /// <summary>
    /// The Data Type.
    /// </summary>
    public enum DataType : uint
    {
        Byte = GL.GL_BYTE,
        UnsignedByte = GL.GL_UNSIGNED_BYTE,
        Short = GL.GL_SHORT,
        UnsignedShort = GL.GL_UNSIGNED_SHORT,
        Int = GL.GL_INT,
        UnsignedInt = GL.GL_UNSIGNED_INT,
        Float = GL.GL_FLOAT,
        TwoBytes = GL.GL_2_BYTES,
        ThreeBytes = GL.GL_3_BYTES,
        FourBytes = GL.GL_4_BYTES,
        /// <summary>
        /// 
        /// </summary>
        Double= GL.GL_DOUBLE
    }

    /// <summary>
    /// The depth function
    /// </summary>
    public enum DepthFunction : uint
    {
        Never = GL.GL_NEVER,
        Less = GL.GL_LESS,
        Equal = GL.GL_EQUAL,
        LessThanOrEqual = GL.GL_LEQUAL,
        Great = GL.GL_GREATER,
        NotEqual = GL.GL_NOTEQUAL,
        GreaterThanOrEqual = GL.GL_GEQUAL,
        Always = GL.GL_ALWAYS,
    }

    /// <summary>
    /// The Draw Buffer Mode
    /// </summary>
    public enum DrawBufferMode : uint
    {
        None = GL.GL_NONE,
        FrontLeft = GL.GL_FRONT_LEFT,
        FrontRight = GL.GL_FRONT_RIGHT,
        BackLeft = GL.GL_BACK_LEFT,
        BackRight = GL.GL_BACK_RIGHT,
        Front = GL.GL_FRONT,
        Back = GL.GL_BACK,
        Left = GL.GL_LEFT,
        Right = GL.GL_RIGHT,
        FrontAndBack = GL.GL_FRONT_AND_BACK,
        Auxilliary0= GL.GL_AUX0,
        Auxilliary1 = GL.GL_AUX1,
        Auxilliary2 = GL.GL_AUX2,
        Auxilliary3 = GL.GL_AUX3,
    }
    
    /// <summary>
    /// Error Code
    /// </summary>
    public enum ErrorCode : uint
    {
        NoError = GL.GL_NO_ERROR,
        InvalidEnum = GL.GL_INVALID_ENUM,
        InvalidValue = GL.GL_INVALID_VALUE,
        InvalidOperation = GL.GL_INVALID_OPERATION,
        StackOverflow = GL.GL_STACK_OVERFLOW,
        StackUnderflow = GL.GL_STACK_UNDERFLOW,
        OutOfMemory = GL.GL_OUT_OF_MEMORY
    }

    /// <summary>
    /// FeedBackMode
    /// </summary>
    public enum FeedbackMode : uint
    {
        TwoD = GL.GL_2D,
        ThreeD = GL.GL_3D,
        FourD = GL.GL_4D_COLOR,
        ThreeDColorTexture = GL.GL_3D_COLOR_TEXTURE,
        FourDColorTexture = GL.GL_4D_COLOR_TEXTURE
    }

    /// <summary>
    /// The Feedback Token
    /// </summary>
    public enum FeedbackToken : uint
    {
        PassThroughToken = GL.GL_PASS_THROUGH_TOKEN,
        PointToken = GL.GL_POINT_TOKEN,
        LineToken = GL.GL_LINE_TOKEN,
        PolygonToken = GL.GL_POLYGON_TOKEN,
        BitmapToken = GL.GL_BITMAP_TOKEN,
        DrawPixelToken = GL.GL_DRAW_PIXEL_TOKEN,
        CopyPixelToken = GL.GL_COPY_PIXEL_TOKEN,
        LineResetToken = GL.GL_LINE_RESET_TOKEN
    }

    /// <summary>
    /// The Fog Mode.
    /// </summary>
    public enum FogMode : uint
    {
	   	Exp = GL.GL_EXP,

        /// <summary>
        /// 
        /// </summary>
		Exp2 = GL.GL_EXP2,
	}
	
    /// <summary>
    /// GetMapTarget 
    /// </summary>
    public enum GetMapTarget : uint
    {
        Coeff = GL.GL_COEFF,
        Order = GL.GL_ORDER,
        Domain = GL.GL_DOMAIN
    }

    public enum GetTarget : uint
    {
        CurrentColor = GL.GL_CURRENT_COLOR,
        CurrentIndex = GL.GL_CURRENT_INDEX,
        CurrentNormal = GL.GL_CURRENT_NORMAL,
        CurrentTextureCoords = GL.GL_CURRENT_TEXTURE_COORDS,
        CurrentRasterColor = GL.GL_CURRENT_RASTER_COLOR,
        CurrentRasterIndex = GL.GL_CURRENT_RASTER_INDEX,
        CurrentRasterTextureCoords = GL.GL_CURRENT_RASTER_TEXTURE_COORDS,
        CurrentRasterPosition = GL.GL_CURRENT_RASTER_POSITION,
        CurrentRasterPositionValid = GL.GL_CURRENT_RASTER_POSITION_VALID,
        CurrentRasterDistance = GL.GL_CURRENT_RASTER_DISTANCE,
        PointSmooth = GL.GL_POINT_SMOOTH,
        PointSize = GL.GL_POINT_SIZE,
        PointSizeRange = GL.GL_POINT_SIZE_RANGE,
        PointSizeGranularity = GL.GL_POINT_SIZE_GRANULARITY,
        LineSmooth = GL.GL_LINE_SMOOTH,
        LineWidth = GL.GL_LINE_WIDTH,
        LineWidthRange = GL.GL_LINE_WIDTH_RANGE,
        LineWidthGranularity = GL.GL_LINE_WIDTH_GRANULARITY,
        LineStipple = GL.GL_LINE_STIPPLE,
        LineStipplePattern = GL.GL_LINE_STIPPLE_PATTERN,
        LineStippleRepeat = GL.GL_LINE_STIPPLE_REPEAT,
        ListMode = GL.GL_LIST_MODE,
        MaxListNesting = GL.GL_MAX_LIST_NESTING,
        ListBase = GL.GL_LIST_BASE,
        ListIndex = GL.GL_LIST_INDEX,
        PolygonMode = GL.GL_POLYGON_MODE,
        PolygonSmooth = GL.GL_POLYGON_SMOOTH,
        PolygonStipple = GL.GL_POLYGON_STIPPLE,
        EdgeFlag = GL.GL_EDGE_FLAG,
        CullFace = GL.GL_CULL_FACE,
        CullFaceMode = GL.GL_CULL_FACE_MODE,
        FrontFace = GL.GL_FRONT_FACE,
        Lighting = GL.GL_LIGHTING,
        LightModelLocalViewer = GL.GL_LIGHT_MODEL_LOCAL_VIEWER,
        LightModelTwoSide = GL.GL_LIGHT_MODEL_TWO_SIDE,
        LightModelAmbient = GL.GL_LIGHT_MODEL_AMBIENT,
        ShadeModel = GL.GL_SHADE_MODEL,
        ColorMaterialFace = GL.GL_COLOR_MATERIAL_FACE,
        ColorMaterialParameter = GL.GL_COLOR_MATERIAL_PARAMETER,
        ColorMaterial = GL.GL_COLOR_MATERIAL,
        Fog = GL.GL_FOG,
        FogIndex = GL.GL_FOG_INDEX,
        FogDensity = GL.GL_FOG_DENSITY,
        FogStart = GL.GL_FOG_START,
        FogEnd = GL.GL_FOG_END,
        FogMode = GL.GL_FOG_MODE,
        FogColor = GL.GL_FOG_COLOR,
        DepthRange = GL.GL_DEPTH_RANGE,
        DepthTest = GL.GL_DEPTH_TEST,
        DepthWritemask = GL.GL_DEPTH_WRITEMASK,
        DepthClearValue = GL.GL_DEPTH_CLEAR_VALUE,
        DepthFunc = GL.GL_DEPTH_FUNC,
        AccumClearValue = GL.GL_ACCUM_CLEAR_VALUE,
        StencilTest = GL.GL_STENCIL_TEST,
        StencilClearValue = GL.GL_STENCIL_CLEAR_VALUE,
        StencilFunc = GL.GL_STENCIL_FUNC,
        StencilValueMask = GL.GL_STENCIL_VALUE_MASK,
        StencilFail = GL.GL_STENCIL_FAIL,
        StencilPassDepthFail = GL.GL_STENCIL_PASS_DEPTH_FAIL,
        StencilPassDepthPass = GL.GL_STENCIL_PASS_DEPTH_PASS,
        StencilRef = GL.GL_STENCIL_REF,
        StencilWritemask = GL.GL_STENCIL_WRITEMASK,
        MatrixMode = GL.GL_MATRIX_MODE,
        Normalize = GL.GL_NORMALIZE,
        Viewport = GL.GL_VIEWPORT,
        ModelviewStackDepth = GL.GL_MODELVIEW_STACK_DEPTH,
        ProjectionStackDepth = GL.GL_PROJECTION_STACK_DEPTH,
        TextureStackDepth = GL.GL_TEXTURE_STACK_DEPTH,
        ModelviewMatix = GL.GL_MODELVIEW_MATRIX,
        ProjectionMatrix = GL.GL_PROJECTION_MATRIX,
        TextureMatrix = GL.GL_TEXTURE_MATRIX,
        AttribStackDepth = GL.GL_ATTRIB_STACK_DEPTH,
        ClientAttribStackDepth = GL.GL_CLIENT_ATTRIB_STACK_DEPTH,
        AlphaTest = GL.GL_ALPHA_TEST,
        AlphaTestFunc = GL.GL_ALPHA_TEST_FUNC,
        AlphaTestRef = GL.GL_ALPHA_TEST_REF,
        Dither = GL.GL_DITHER,
        BlendDst = GL.GL_BLEND_DST,
        BlendSrc = GL.GL_BLEND_SRC,
        Blend = GL.GL_BLEND,
        LogicOpMode = GL.GL_LOGIC_OP_MODE,
        IndexLogicOp = GL.GL_INDEX_LOGIC_OP,
        ColorLogicOp = GL.GL_COLOR_LOGIC_OP,
        AuxBuffers = GL.GL_AUX_BUFFERS,
        DrawBuffer = GL.GL_DRAW_BUFFER,
        ReadBuffer = GL.GL_READ_BUFFER,
        ScissorBox = GL.GL_SCISSOR_BOX,
        ScissorTest = GL.GL_SCISSOR_TEST,
        IndexClearValue = GL.GL_INDEX_CLEAR_VALUE,
        IndexWritemask = GL.GL_INDEX_WRITEMASK,
        ColorClearValue = GL.GL_COLOR_CLEAR_VALUE,
        ColorWritemask = GL.GL_COLOR_WRITEMASK,
        IndexMode = GL.GL_INDEX_MODE,
        RgbaMode = GL.GL_RGBA_MODE,
        DoubleBuffer = GL.GL_DOUBLEBUFFER,
        Stereo = GL.GL_STEREO,
        RenderMode = GL.GL_RENDER_MODE,
        PerspectiveCorrectionHint = GL.GL_PERSPECTIVE_CORRECTION_HINT,
        PointSmoothHint = GL.GL_POINT_SMOOTH_HINT,
        LineSmoothHint = GL.GL_LINE_SMOOTH_HINT,
        PolygonSmoothHint = GL.GL_POLYGON_SMOOTH_HINT,
        FogHint = GL.GL_FOG_HINT,
        TextureGenS = GL.GL_TEXTURE_GEN_S,
        TextureGenT = GL.GL_TEXTURE_GEN_T,
        TextureGenR = GL.GL_TEXTURE_GEN_R,
        TextureGenQ = GL.GL_TEXTURE_GEN_Q,
        PixelMapItoI = GL.GL_PIXEL_MAP_I_TO_I,
        PixelMapStoS = GL.GL_PIXEL_MAP_S_TO_S,
        PixelMapItoR = GL.GL_PIXEL_MAP_I_TO_R,
        PixelMapItoG = GL.GL_PIXEL_MAP_I_TO_G,
        PixelMapItoB = GL.GL_PIXEL_MAP_I_TO_B,
        PixelMapItoA = GL.GL_PIXEL_MAP_I_TO_A,
        PixelMapRtoR = GL.GL_PIXEL_MAP_R_TO_R,
        PixelMapGtoG = GL.GL_PIXEL_MAP_G_TO_G,
        PixelMapBtoB = GL.GL_PIXEL_MAP_B_TO_B,
        PixelMapAtoA = GL.GL_PIXEL_MAP_A_TO_A,
        PixelMapItoISize = GL.GL_PIXEL_MAP_I_TO_I_SIZE,
        PixelMapStoSSize = GL.GL_PIXEL_MAP_S_TO_S_SIZE,
        PixelMapItoRSize = GL.GL_PIXEL_MAP_I_TO_R_SIZE,
        PixelMapItoGSize = GL.GL_PIXEL_MAP_I_TO_G_SIZE,
        PixelMapItoBSize = GL.GL_PIXEL_MAP_I_TO_B_SIZE,
        PixelMapItoASize = GL.GL_PIXEL_MAP_I_TO_A_SIZE,
        PixelMapRtoRSize = GL.GL_PIXEL_MAP_R_TO_R_SIZE,
        PixelMapGtoGSize = GL.GL_PIXEL_MAP_G_TO_G_SIZE,
        PixelMapBtoBSize = GL.GL_PIXEL_MAP_B_TO_B_SIZE,
        PixelMapAtoASize = GL.GL_PIXEL_MAP_A_TO_A_SIZE,
        UnpackSwapBytes = GL.GL_UNPACK_SWAP_BYTES,
        LsbFirst = GL.GL_UNPACK_LSB_FIRST,
        UnpackRowLength = GL.GL_UNPACK_ROW_LENGTH,
        UnpackSkipRows = GL.GL_UNPACK_SKIP_ROWS,
        UnpackSkipPixels = GL.GL_UNPACK_SKIP_PIXELS,
        UnpackAlignment = GL.GL_UNPACK_ALIGNMENT,
        PackSwapBytes = GL.GL_PACK_SWAP_BYTES,
        PackLsbFirst = GL.GL_PACK_LSB_FIRST,
        PackRowLength = GL.GL_PACK_ROW_LENGTH,
        PackSkipRows = GL.GL_PACK_SKIP_ROWS,
        PackSkipPixels = GL.GL_PACK_SKIP_PIXELS,
        PackAlignment = GL.GL_PACK_ALIGNMENT,
        MapColor = GL.GL_MAP_COLOR,
        MapStencil = GL.GL_MAP_STENCIL,
        IndexShift = GL.GL_INDEX_SHIFT,
        IndexOffset = GL.GL_INDEX_OFFSET,
        RedScale = GL.GL_RED_SCALE,
        RedBias = GL.GL_RED_BIAS,
        ZoomX = GL.GL_ZOOM_X,
        ZoomY = GL.GL_ZOOM_Y,
        GreenScale = GL.GL_GREEN_SCALE,
        GreenBias = GL.GL_GREEN_BIAS,
        BlueScale = GL.GL_BLUE_SCALE,
        BlueBias = GL.GL_BLUE_BIAS,
        AlphaScale = GL.GL_ALPHA_SCALE,
        AlphaBias = GL.GL_ALPHA_BIAS,
        DepthScale = GL.GL_DEPTH_SCALE,
        DepthBias = GL.GL_DEPTH_BIAS,
        MapEvalOrder = GL.GL_MAX_EVAL_ORDER,
        MaxLights = GL.GL_MAX_LIGHTS,
        MaxClipPlanes = GL.GL_MAX_CLIP_PLANES,
        MaxTextureSize = GL.GL_MAX_TEXTURE_SIZE,
        MapPixelMapTable = GL.GL_MAX_PIXEL_MAP_TABLE,
        MaxAttribStackDepth = GL.GL_MAX_ATTRIB_STACK_DEPTH,
        MaxModelviewStackDepth = GL.GL_MAX_MODELVIEW_STACK_DEPTH,
        MaxNameStackDepth = GL.GL_MAX_NAME_STACK_DEPTH,
        MaxProjectionStackDepth = GL.GL_MAX_PROJECTION_STACK_DEPTH,
        MaxTextureStackDepth = GL.GL_MAX_TEXTURE_STACK_DEPTH,
        MaxViewportDims = GL.GL_MAX_VIEWPORT_DIMS,
        MaxClientAttribStackDepth = GL.GL_MAX_CLIENT_ATTRIB_STACK_DEPTH,
        SubpixelBits = GL.GL_SUBPIXEL_BITS,
        IndexBits = GL.GL_INDEX_BITS,
        RedBits = GL.GL_RED_BITS,
        GreenBits = GL.GL_GREEN_BITS,
        BlueBits = GL.GL_BLUE_BITS,
        AlphaBits = GL.GL_ALPHA_BITS,
        DepthBits = GL.GL_DEPTH_BITS,
        StencilBits = GL.GL_STENCIL_BITS,
        AccumRedBits = GL.GL_ACCUM_RED_BITS,
        AccumGreenBits = GL.GL_ACCUM_GREEN_BITS,
        AccumBlueBits = GL.GL_ACCUM_BLUE_BITS,
        AccumAlphaBits = GL.GL_ACCUM_ALPHA_BITS,
        NameStackDepth = GL.GL_NAME_STACK_DEPTH,
        AutoNormal = GL.GL_AUTO_NORMAL,
        Map1Color4 = GL.GL_MAP1_COLOR_4,
        Map1Index = GL.GL_MAP1_INDEX,
        Map1Normal = GL.GL_MAP1_NORMAL,
        Map1TextureCoord1 = GL.GL_MAP1_TEXTURE_COORD_1,
        Map1TextureCoord2 = GL.GL_MAP1_TEXTURE_COORD_2,
        Map1TextureCoord3 = GL.GL_MAP1_TEXTURE_COORD_3,
        Map1TextureCoord4 = GL.GL_MAP1_TEXTURE_COORD_4,
        Map1Vertex3 = GL.GL_MAP1_VERTEX_3,
        Map1Vertex4 = GL.GL_MAP1_VERTEX_4,
        Map2Color4 = GL.GL_MAP2_COLOR_4,
        Map2Index = GL.GL_MAP2_INDEX,
        Map2Normal = GL.GL_MAP2_NORMAL,
        Map2TextureCoord1 = GL.GL_MAP2_TEXTURE_COORD_1,
        Map2TextureCoord2 = GL.GL_MAP2_TEXTURE_COORD_2,
        Map2TextureCoord3 = GL.GL_MAP2_TEXTURE_COORD_3,
        Map2TextureCoord4 = GL.GL_MAP2_TEXTURE_COORD_4,
        Map2Vertex3 = GL.GL_MAP2_VERTEX_3,
        Map2Vertex4 = GL.GL_MAP2_VERTEX_4,
        Map1GridDomain = GL.GL_MAP1_GRID_DOMAIN,
        Map1GridSegments = GL.GL_MAP1_GRID_SEGMENTS,
        Map2GridDomain = GL.GL_MAP2_GRID_DOMAIN,
        Map2GridSegments = GL.GL_MAP2_GRID_SEGMENTS,
        Texture1D = GL.GL_TEXTURE_1D,
        Texture2D = GL.GL_TEXTURE_2D,
        FeedbackBufferPointer = GL.GL_FEEDBACK_BUFFER_POINTER,
        FeedbackBufferSize = GL.GL_FEEDBACK_BUFFER_SIZE,
        FeedbackBufferType = GL.GL_FEEDBACK_BUFFER_TYPE,
        SelectionBufferPointer = GL.GL_SELECTION_BUFFER_POINTER,
        SelectionBufferSize = GL.GL_SELECTION_BUFFER_SIZE
    }

    public enum GLType : uint
    {
        Int = GL.GL_INT,
        Float = GL.GL_FLOAT,
        FloatMat4 = GL.GL_FLOAT_MAT4,
        FloatVec2 = GL.GL_FLOAT_VEC2,
        FloatVec3 = GL.GL_FLOAT_VEC3,
        FloatVec4 = GL.GL_FLOAT_VEC4,
        Sampler2D = GL.GL_SAMPLER_2D,
    }

    /// <summary>
    /// The Front Face Mode.
    /// </summary>
    public enum FrontFaceMode : uint
    {
        ClockWise = GL.GL_CW,
        CounterClockWise = GL.GL_CCW,
    }


    /// <summary>
    /// The hint mode.
    /// </summary>
	public enum HintMode : uint
    {
		DontCare = GL.GL_DONT_CARE,
		Fastest = GL.GL_FASTEST,
        /// <summary>
        /// The 
        /// </summary>
        Nicest = GL.GL_NICEST
    }

    /// <summary>
    /// The hint target.
    /// </summary>
    public enum HintTarget : uint
    {
        PerspectiveCorrection = GL.GL_PERSPECTIVE_CORRECTION_HINT,
        PointSmooth = GL.GL_POINT_SMOOTH_HINT,
        LineSmooth = GL.GL_LINE_SMOOTH_HINT,
        PolygonSmooth = GL.GL_POLYGON_SMOOTH_HINT,
        Fog = GL.GL_FOG_HINT
    }
     
    /// <summary>
    /// LightName
    /// </summary>
    public enum LightName : uint
    {
		Light0 = GL.GL_LIGHT0  ,
        Light1 = GL.GL_LIGHT1,
        Light2 = GL.GL_LIGHT2,
        Light3 = GL.GL_LIGHT3,
        Light4 = GL.GL_LIGHT4,
        Light5 = GL.GL_LIGHT5,
        Light6 = GL.GL_LIGHT6,
        Light7 = GL.GL_LIGHT7  
    }
	
    /// <summary>
    /// LightParameter
    /// </summary>
    public enum LightParameter : uint
    {
        Ambient = GL.GL_AMBIENT,
        Diffuse = GL.GL_DIFFUSE,
        Specular = GL.GL_SPECULAR,
        Position = GL.GL_POSITION,
        SpotDirection = GL.GL_SPOT_DIRECTION,
        SpotExponent = GL.GL_SPOT_EXPONENT,
        SpotCutoff = GL.GL_SPOT_CUTOFF,
        ConstantAttenuatio = GL.GL_CONSTANT_ATTENUATION,
        LinearAttenuation = GL.GL_LINEAR_ATTENUATION,
        QuadraticAttenuation = GL.GL_QUADRATIC_ATTENUATION
    }

    /// <summary>
    /// The Light Model Parameter.
    /// </summary>
    public enum LightModelParameter : uint
    {
        LocalViewer = GL.GL_LIGHT_MODEL_LOCAL_VIEWER,
        TwoSide = GL.GL_LIGHT_MODEL_TWO_SIDE,
        Ambient = GL.GL_LIGHT_MODEL_AMBIENT
    }

    /// <summary>
    /// The Logic Op
    /// </summary>
    public enum LogicOp : uint
    {
        Clear = GL.GL_CLEAR,
        And = GL.GL_AND,
        AndReverse  = GL.GL_AND_REVERSE,
        Copy = GL.GL_COPY,
        AndInverted = GL.GL_AND_INVERTED,
        NoOp= GL.GL_NOOP,
        XOr = GL.GL_XOR,
        Or = GL.GL_OR,
        NOr= GL.GL_NOR,
        Equiv = GL.GL_EQUIV,
        Invert = GL.GL_INVERT,
        OrReverse = GL.GL_OR_REVERSE,
        CopyInverted = GL.GL_COPY_INVERTED,
        OrInverted = GL.GL_OR_INVERTED,
        NAnd= GL.GL_NAND,
        Set = GL.GL_SET,
    }

    /// <summary>
    /// The matrix mode.
    /// </summary>
    public enum MatrixMode : uint
    {
        Modelview = GL.GL_MODELVIEW,
        Projection = GL.GL_PROJECTION,
        Texture = GL.GL_TEXTURE
    }

    /// <summary>
    /// The pixel transfer parameter name
    /// </summary>
    public enum PixelTransferParameterName : uint
    {
        MapColor = GL.GL_MAP_COLOR,
        MapStencil = GL.GL_MAP_STENCIL,
        IndexShift = GL.GL_INDEX_SHIFT,
        IndexOffset = GL.GL_INDEX_OFFSET,
        RedScale = GL.GL_RED_SCALE,
        RedBias = GL.GL_RED_BIAS,
        ZoomX = GL.GL_ZOOM_X,
        ZoomY = GL.GL_ZOOM_Y,
        GreenScale = GL.GL_GREEN_SCALE,
        GreenBias = GL.GL_GREEN_BIAS,
        BlueScale = GL.GL_BLUE_SCALE,
        BlueBias = GL.GL_BLUE_BIAS,
        AlphaScale = GL.GL_ALPHA_SCALE,
        AlphaBias = GL.GL_ALPHA_BIAS,
        DepthScale = GL.GL_DEPTH_SCALE,
        DepthBias = GL.GL_DEPTH_BIAS
    }

    /// <summary>
    /// The Polygon mode.
    /// </summary>
    public enum PolygonMode : uint
    {
        /// <summary>
        /// Render as points.
        /// </summary>
        Points = GL.GL_POINT,

        /// <summary>
        /// Render as lines.
        /// </summary>
        Lines = GL.GL_LINE,

        /// <summary>
        /// Render as filled.
        /// </summary>
        Filled = GL.GL_FILL
    }
    
    /// <summary>
    /// Rendering Mode 
    /// </summary>
    public enum RenderingMode: uint
    {
        Render = GL.GL_RENDER,
        Feedback = GL.GL_FEEDBACK,
        Select = GL.GL_SELECT
    }

    /// <summary>
    /// ShadingModel
    /// </summary>
	public enum ShadeModel : uint
    {
        Flat = GL.GL_FLAT,
        Smooth = GL.GL_SMOOTH
    }

    public enum ShaderType : uint
    {
        Vertex = GL.GL_VERTEX_SHADER,
        Fragment = GL.GL_FRAGMENT_SHADER,
        Geometry = GL.GL_GEOMETRY_SHADER,
    }

    public enum ShaderStage : uint
    {
        Vertex = GL.GL_VERTEX_SHADER_BIT,
        Fragment = GL.GL_FRAGMENT_SHADER_BIT,
        Geometry = GL.GL_GEOMETRY_SHADER_BIT,
    }

    /// <summary>
    /// The stencil function
    /// </summary>
    public enum StencilFunction : uint
    {
        Never = GL.GL_NEVER,
        Less = GL.GL_LESS,
        Equal = GL.GL_EQUAL,
        LessThanOrEqual = GL.GL_LEQUAL,
        Great = GL.GL_GREATER,
        NotEqual = GL.GL_NOTEQUAL,
        GreaterThanOrEqual = GL.GL_GEQUAL,
        Always = GL.GL_ALWAYS,
    }

    /// <summary>
    /// The stencil operation.
    /// </summary>
    public enum StencilOperation : uint
    {
        Keep = GL.GL_KEEP,
        Replace = GL.GL_REPLACE,
        Increase = GL.GL_INCR,
        Decrease = GL.GL_DECR,
        Zero = GL.GL_ZERO,
        IncreaseWrap = GL.GL_INCR_WRAP,
        DecreaseWrap = GL.GL_DECR_WRAP,
        Invert = GL.GL_INVERT
    }    
    
    /// <summary>
    /// GetTextureParameter
    /// </summary>
    public enum TextureParameter : uint
    {
        TextureWidth = GL.GL_TEXTURE_WIDTH,
        TextureHeight = GL.GL_TEXTURE_HEIGHT,
        TextureInternalFormat = GL.GL_TEXTURE_INTERNAL_FORMAT,
        TextureBorderColor = GL.GL_TEXTURE_BORDER_COLOR,
        TextureBorder = GL.GL_TEXTURE_BORDER
    }

    /// <summary>
    /// Texture target.
    /// </summary>
    public enum TextureTarget : uint
    {
        Texture1D = GL.GL_TEXTURE_1D,
        Texture2D = GL.GL_TEXTURE_2D,
        Texture3D = GL.GL_TEXTURE_3D
    }

    public enum Usage : uint
    {
        DynamicDraw = GL.GL_DYNAMIC_DRAW,
        StaticDraw = GL.GL_STATIC_DRAW,
    }
}
