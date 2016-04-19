using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;

//--------------------------------------------------------------------------------------
//@author	KONICA MINOLTA, INC.
//		    Copyright(c) KONICA MINOLTA, INC.  All rights reserved. 2013
//---------------------------------------------------------------------------------------
//   int8_km = byte
//   int32_km = Integer
//   int16_km = Short
//   double_km = double
//   float_km = Single
//
public class clCA2DSDK
{
    //------------------------------------------const error----------------------------------------------
    //!< Processing was canceled
    public const int CA2D_CANCEL = 0;
    //!< Processing completed normally
    public const int CA2D_OK = 1;

    //!< Measuring
    public const int CA2D_OK_MEASURING = 10;
    //!< Running self-diagnostic
    public const int CA2D_OK_RUNNING_DIAGNOSIS = 11;

    //!< A problem occurred in the SDK
    public const int CA2D_ER = -1;

    //!< Initialization is not complete (Initialization failed)
    public const int CA2D_ER_ENABLE = -1000;
    //!< Connection is not complete (Connection failed)
    public const int CA2D_ER_CONNECT = -1001;

    //!< The instrument information is incorrect
    public const int CA2D_ER_INSTRUMENT = -1050;

    //!< Cannot be processed because the measurement is running
    public const int CA2D_ER_MEASURING = -1100;
    //!< Processing is not possible because the finder is running
    public const int CA2D_ER_RUNNING_FINDER = -1101;
    //!< Cannot be processed because the instrument diagnostic is running
    public const int CA2D_ER_RUNNING_DIAGNOSIS = -1102;

    //<! No measurement data (not measured or measurement error)
    public const int CA2D_ER_DATA_NONE = -1150;

    //!< Not an appropriate exposure time
    public const int CA2D_ER_EXPOSURE = -1200;
    //!< Failed to calculate the exposure time (overexposed)
    public const int CA2D_ER_AUTOEXPO_OVER = -1201;
    //!< Failed to calculate the exposure time (underexposed)
    public const int CA2D_ER_AUTOEXPO_UNDER = -1202;

    //!< Temperature error
    public const int CA2D_ER_TEMPERATURE = -1250;

    //!< User calibration data has not been set
    public const int CA2D_ER_USERCAL_NONE = -1300;
    //!< The user calibration data is invalid (calibration factor could not be calculated)
    public const int CA2D_ER_USER_CALC = -1301;

    //!< Periodic calibration is now required
    public const int CA2D_ER_PERIODICAL_CAL = -1350;

    //!< Stopped due to an error that occurred during the diagnostic
    public const int CA2D_ER_DIAGNOSIS = -1400;
    //!< There is a problem with the diagnostic results
    public const int CA2D_ER_DIAGNOSIS_AREA = -1401;
    //!< The diagnostic results have reached the caution level
    public const int CA2D_ER_DIAGNOSIS_CAUTION = -1402;
    //!< The diagnostic results have reached the warning level
    public const int CA2D_ER_DIAGNOSIS_WARNING = -1403;

    //!< The lens type file does not exist
    public const int CA2D_ER_FILE_NOTFOUND_LENSTYPE = -1500;
    //!< Calibration data file does not exist
    public const int CA2D_ER_FILE_NOTFOUND_CALIBRATION = -1501;
    //!< Invalid file format
    public const int CA2D_ER_FILE_FORMAT = -1502;
    //!< Could not access the file
    public const int CA2D_ER_FILE_ACCESS = -1503;

    //!< Invalid argument specification
    public const int CA2D_ER_PARAM = -1600;
    //!< Invalid instrument specification (the specified instrument does not exist)
    public const int CA2D_ER_PARAM_INDEX = -1610;

    //!< Invalid lens type specification (lens cannot be used)
    public const int CA2D_ER_PARAM_LENSTYPE = -1620;
    //!< Invalid focus ring distance indicator specification
    public const int CA2D_ER_PARAM_LENSPOS = -1621;
    //!< Invalid exposure mode specification
    public const int CA2D_ER_PARAM_EXPOSURE_MODE = -1622;
    //!< Invalid exposure table index number specification
    public const int CA2D_ER_PARAM_EXPOSURE_INDEX = -1623;
    //!< Invalid exposure setting area setting
    public const int CA2D_ER_PARAM_EXPOSURE_AREA = -1624;
    //!< Invalid synchronized measurement mode specification
    public const int CA2D_ER_PARAM_SYNC_MODE = -1625;
    //!< Invalid synchronization frequency specification
    public const int CA2D_ER_PARAM_SYNC_VALUE = -1626;
    //!< Invalid cumulative number specification
    public const int CA2D_ER_PARAM_ADDITIONAL = -1627;
    //!< Invalid filter measurement specification
    public const int CA2D_ER_PARAM_FILTER = -1628;
    //!< Invalid filter type specification
    public const int CA2D_ER_PARAM_FILTER_INDEX = -1629;
    //!< Invalid smear compensation specification
    public const int CA2D_ER_PARAM_SMEAR = -1630;
    //!< Invalid user calibration specification
    public const int CA2D_ER_PARAM_USERCAL = -1631;
    //!< Invalid image orientation specification
    public const int CA2D_ER_PARAM_ROTATION = -1632;

    //!< Invalid file path (folder path)
    public const int CA2D_ER_PARAM_FILE = -1640;
    //!< Invalid calibration type specification
    public const int CA2D_ER_PARAM_CALTYPE = -1650;
    //!< Invalid data acquisition area specification
    public const int CA2D_ER_PARAM_GETAREA = -1660;

    //!< Invalid color specification value specification
    public const int CA2D_ER_PARAM_VALTYPE = -1670;
    //!< Invalid resolution specification
    public const int CA2D_ER_PARAM_RESOLUTION = -1671;
    //!< Invalid threshold value specification to process as an under error
    public const int CA2D_ER_PARAM_LOWER_LEVEL = -1672;

    //!< Invalid layout type specification
    public const int CA2D_ER_PARAM_EVAL_TYPE = -1680;
    //!< Invalid area specification
    public const int CA2D_ER_PARAM_EVAL_AREA = -1681;
    //!< Invalid detection count specification
    public const int CA2D_ER_PARAM_EVAL_COUNT = -1682;
    //!< Invalid detection threshold value specification
    public const int CA2D_ER_PARAM_EVAL_LEVEL = -1683;

    //!< Invalid layout count specification
    public const int CA2D_ER_PARAM_SPOT_COUNT = -1690;
    //!< Invalid shape specification
    public const int CA2D_ER_PARAM_SPOT_SHAPE = -1691;
    //!< Invalid size specification
    public const int CA2D_ER_PARAM_SPOT_SIZE = -1692;
    //!< Invalid offset input method specification
    public const int CA2D_ER_PARAM_SPOT_OFFSET_INPUT = -1693;
    //!< Invalid offset layout position specification
    public const int CA2D_ER_PARAM_SPOT_OFFSET_POS = -1694;
    //!< Invalid offset margin specification
    public const int CA2D_ER_PARAM_SPOT_OFFSET_AREA = -1695;

    //!< Invalid evaluation area number specification
    public const int CA2D_ER_PARAM_NUM = -1700;
    //!< Invalid spot area specification
    public const int CA2D_ER_PARAM_POINT = -1701;

    //!< Could not correctly layout the evaluation areas
    public const int CA2D_ER_EVAL_CALC = -1710;
    //!< Could not correctly lay out the spots
    public const int CA2D_ER_SPOT_CALC = -1711;

    //!< Measurement calculation failed
    public const int CA2D_ER_MEASURE_CALC = -1800;


    //!< Could not communicate with the instrument
    public const int CA2D_ER_COMMUNICATION = -5000;
    //!< Filter control is not operating correctly
    public const int CA2D_ER_COM_FILTER = -5010;
    //!< Get status is not operating correctly
    public const int CA2D_ER_COM_STATUS = -5020;
    //!< Measurement control is not operating correctly
    public const int CA2D_ER_COM_MEASURE = -5030;
    //!< Finder measurement is not operating correctly
    public const int CA2D_ER_COM_FINDER = -5040;
    //!< Shutter speed control is not operating correctly
    public const int CA2D_ER_COM_SHUTTERSPEED = -5050;
    //!< Gain control is not operating correctly
    public const int CA2D_ER_COM_GAIN = -5060;
    //!< Cumulative number control is not operating correctly
    public const int CA2D_ER_COM_ADDTIONAL = -5070;
    //!< Data acquisition control is not operating correctly
    public const int CA2D_ER_COM_GETDATA = -5110;
    //!< emperature control is not operating correctly
    public const int CA2D_ER_COM_TEMPERATURE = -5120;
    //!< Fan control is not operating correctly
    public const int CA2D_ER_COM_FAN = -5130;
    //!< Motor control is not operating correctly
    public const int CA2D_ER_COM_MOTOR = -5140;
    //!< Insufficient memory
    public const int CA2D_ER_COM_MEMORY = -5150;
    //!< The instrument was disconnected
    public const int CA2D_ER_COM_DETECT = -5200;
    //!< USB driver control is not operating correctly
    public const int CA2D_ER_USBDLL = -5300;

    //praivate info.
    public const int CA2D_ER_COM_BINNING = -5080;
    public const int CA2D_ER_COM_EXAMINATIONAREA = -5090;

    public const int CA2D_ER_COM_CCDMODE = -5100;
    //------------------------------------------common---------------------------------------------------

    public const int MAX_CONNECT_LIST = 10;
    //row size max.
    public const int MAXDATAROW = 980;
    //!< col size min.
    public const int MAXDATACOL = 980;

    //!< row size for finder
    public const int FINDER_MAXDATAROW = 490;
    //!< col size for finder
    public const int FINDER_MAXDATACOL = 490;

    //!< Lens type : Normal lens
    public const int LENS_NORMAL = 0;
    //!< Lens type : Wide lens
    public const int LENS_WIDE = 1;
    //!< Lens type : Telephoto lens
    public const int LENS_TELE = 2;
    //!< Lens type : Macro 1 (low magnification)
    public const int LENS_MACRO1 = 3;
    //!< Lens type : Macro 2 (high magnification)
    public const int LENS_MACRO2 = 4;
    //!< Lens count
    public const int LENS_NUM = 5;

    //Lens position						            Normal		Wide	Telephoto	Macro1	Macro2
    //0.25m		0.20m	0.90m		0.50m	0.30m
    public const int LENS_POSITION_0 = 0;
    //0.25m+1/2	0.24m	0.90m+1/2
    public const int LENS_POSITION_1 = 1;
    //0.30m		0.30m	1.00m
    public const int LENS_POSITION_2 = 2;
    //0.30m+1/2	0.50m	1.00m+1/3
    public const int LENS_POSITION_3 = 3;
    //0.50m		1.00m	1.00m+2/3
    public const int LENS_POSITION_4 = 4;
    //0.50m+1/2	inf.	1.50m
    public const int LENS_POSITION_5 = 5;
    //1.00m				1.50m+1/3
    public const int LENS_POSITION_6 = 6;
    //1.00m+1/2			1.50m+2/3	
    public const int LENS_POSITION_7 = 7;
    //inf.				3.00m	
    public const int LENS_POSITION_8 = 8;
    //					3.00m+1/3
    public const int LENS_POSITION_9 = 9;
    //					3.00m+2/3
    public const int LENS_POSITION_10 = 10;
    //					inf.
    public const int LENS_POSITION_11 = 11;

    //Exposure mode
    //!< Manual exposure
    public const int EXPOSURE_MANUAL = 0;
    //!< Auto exposure
    public const int EXPOSURE_AUTO = 1;
    //!< Multiple exposure
    public const int EXPOSURE_MULTI = 2;

    //Synchronized measurement frequency
    //!< Synchronized measurements : Do't care
    public const int SYNCMODE_OFF = 0;
    //!< Synchronized measurements
    public const int SYNCMODE_ON = 1;
    //!< Max. frequency
    public const float SYNC_MIN = 4f;
    //!< Min. frequency
    public const float SYNC_MAX = 2000f;

    //Exposure time table
    //!< Min.
    public const int EXPOSURE_INDEX_MIN = 0;
    //!< Max.
    public const int EXPOSURE_INDEX_MAX = 20;

    //Cumulative number 
    //!< Min.
    public const int ADDITIONAL_MIN = 1;
    //!< Max.
    public const int ADDITIONAL_MAX = 256;

    //Exposure setting area
    //!< Min. 
    public const int EXPOSURE_AREA_MIN = 0;
    //!< Max.
    public const int EXPOSURE_AREA_MAX = 979;

    //Measurement component
    //!< XYZ measurements
    public const int FILTER_MEASURE_OFF = 0;
    //!< Filter measuement
    public const int FILTER_MEASURE_ON = 1;

    //Filter index number
    public const int FILTER_INDEX_X = 0;
    public const int FILTER_INDEX_Y = 1;

    public const int FILTER_INDEX_Z = 2;
    //Smear compensation
    //!< None
    public const int SMEAR_NONE = 0;
    //!< Simple compensation
    public const int SMEAR_SIMPLE = 1;
    //!< Approximate compensation
    public const int SMEAR_DETAIL = 2;

    //Apply user calibration
    //!< OFF
    public const int USERCAL_OFF = 0;
    //!< ON
    public const int USERCAL_ON = 1;

    //Image orientation
    //!< Output unmodified
    public const int ROTATION_NONE = 0;
    //!< Rotate clockwise 90
    public const int ROTATION_RIGHT = 1;
    //!< Rotate clockwise 180
    public const int ROTATION_INVERSION = 2;
    //!< Rotate clockwise 270
    public const int ROTATION_LEFT = 3;

    //Calibration type
    //!< No calibration
    public const int USER_CALTYPE_NONE = 0;
    //!< One-color calibration
    public const int USER_CALTYPE_NORMAL = 1;
    //!< RGB calibration
    public const int USER_CALTYPE_RGB = 2;
    //!< WWRGB calibration
    public const int USER_CALTYPE_WRGB = 3;
    //!< Color gamut calibration
    public const int USER_CALTYPE_GAMUT = 4;

    public const int USER_CALTYPE_EDIT = 5;
    //Measurement data error
    //!< Over error pixel
    public const float COUNT_MAX_ERR = -3.302823E+38f;
    //!< Under error pixel
    public const float COUNT_MIN_ERR = -2.402823E+38f;
    //!< Calculation error pixel
    public const float CAL_ERROR = -1.402823E+38f;

    //Color specification value to calculate
    public const int VALTYPE_X = 0;
    public const int VALTYPE_Y = 1;
    public const int VALTYPE_Z = 2;
    public const int VALTYPE_LV = 3;
    public const int VALTYPE_S_X = 4;
    public const int VALTYPE_S_Y = 5;
    public const int VALTYPE_UD = 6;
    public const int VALTYPE_VD = 7;
    public const int VALTYPE_TCP = 8;
    public const int VALTYPE_DUV = 9;
    public const int VALTYPE_TCP_JIS = 10;
    public const int VALTYPE_DUV_JIS = 11;
    //!< Dominant wavelength
    public const int VALTYPE_DWL = 12;
    public const int VALTYPE_PURITY = 13;
    public const int VALTYPE_EV = 14;

    public const int VALTYPE_COUNT = 15;
    //Resolution
    //!< 980*980
    public const int RESOLUTION_HIGH = 0;
    //!< 490*490
    public const int RESOLUTION_MIDDLE = 1;
    //!< 196*196
    public const int RESOLUTION_LOW = 2;

    //Layout method
    //!< Manual layout
    public const int EXTTYPE_MANU = 0;
    //!< Automatic layout
    public const int EXTTYPE_AUTO = 1;

    //Spot shape
    //!< Circle
    public const int SPOT_SHAPETYPE_CIRCLE = 0;
    //!< Rectangle
    public const int SPOT_SHAPETYPE_RECT = 1;

    //Offset input method
    //!< Absolute values
    public const int SPOT_OFFSET_ABSOLUTE = 0;
    //!< Relative values
    public const int SPOT_OFFSET_RELATIVE = 1;

    //Offset position
    //!< Set spot edge as edge
    public const int SPOT_OFFSET_CORNER = 0;
    //!< Set spot center as edge
    public const int SPOT_OFFSET_CENTER = 1;

    //Color values of spot results 
    //!< X, Y, Z
    public const int VALTYPE_X_Y_Z = 0;
    //!< Lv, x, y
    public const int VALTYPE_LV_SX_SY = 1;
    //!< Y, u’, v’
    public const int VALTYPE_Y_UD_VD = 2;
    //!< Y, Tcp, ⊿uv
    public const int VALTYPE_Y_TCP_DUV = 3;
    //!< Y, Tcp(JIS), ⊿uv(JIS)
    public const int VALTYPE_Y_TCP_DUV_JIS = 4;
    //!< Y, 主波長, 刺激純度
    public const int VALTYPE_Y_DWL_PURITY = 5;


    //Measurement conditions structure
    public struct tagInstrumentCond
    {
        //!< Lens type
        public short lensType;
        //!< Lens position
        public short lensPosition;
        //!< Exposure mode
        public short exposureMode;
        //!< Measurement type
        public short measurementType;
        //!< Synchronized measurement frequency
        public double syncValue;
        //!< Exposure time table
        public short exposureIndex;
        //!< Cumulative number
        public short additional;
        //!< Exposure setting area
        public short left;
        public short top;
        public short right;
        public short bottom;
        //!< Measurement component
        public short filterMeasure;
        //!< Filter index number
        public short filterIndex;
        //!< Smear compensation
        public short smearIndex;
        //!< Apply user calibration
        public short userCal;
        //!< Image orientation
        public short rotate;
    }

    //User calibration data structure
    public struct tagUserCalData
    {
        //!< Calibration type
        public short calType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (6))]
        //!< Red data
        public double[] R;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (6))]
        //!< Green data
        public double[] G;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (6))]
        //!< Blue data
        public double[] B;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (6))]
        //!< White data
        public double[] W;
    }

    //Data structure
    public struct tagData
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (clCA2DSDK.MAXDATAROW * clCA2DSDK.MAXDATACOL - 1))]
        public float[] X;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (clCA2DSDK.MAXDATAROW * clCA2DSDK.MAXDATACOL - 1))]
        public float[] Y;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (clCA2DSDK.MAXDATAROW * clCA2DSDK.MAXDATACOL - 1))]
        public float[] Z;
    }

    //Data acquisition conditions structure
    public struct tagGetDataParam
    {
        public short left;
        public short top;
        public short right;
        public short bottom;
    }

    //Data configuration structure
    public struct tagDataCond
    {
        //!< Color specification value to calculate
        public short valueType;
        //!< Resolution
        public short resolution;
        //!< Turn on/off under errors
        public short lower_enable;
        //!< Item of under errors
        public short lower_item;
        //!< The threshold value to process as an under error
        public double lower_threshold;
    }

    //Periodic calibration information structure
    public struct tagPeriodicalCalInfo
    {
        public int Year;
        public int Month;
        public int Day;
        public int Interval;
    }


    //Evaluation area structure
    public struct tagEvaluationArea
    {
        public short left;
        public short top;
        public short right;
        public short bottom;
    }

    //Evaluation area layout conditions structure
    public struct tagEvaluationCond
    {
        //!< Layout method
        public short type;
        //!< Area to detect 
        public short left;
        public short top;
        public short right;
        public short bottom;
        //!< Vertical layout count
        public short row;
        //!< Horizontal layout count
        public short col;
        //!< Threshold value for detection
        public double thresholdValue;
    }

    //Spot layout structure
    public struct tagAlignedSpotCond
    {
        //!< Vertical layout count
        public short row;
        //!< Horizontal layout count
        public short col;
        //!< Shape
        public short shape;
        //!< Rectangle height (or circle diameter)
        public float height;
        //!< Rectangle width
        public float width;
        //!< Offset input method
        public short offset_input;
        //!< Offset position
        public short offset_position;
        //!< Offset margin
        public float offset_left;
        public float offset_top;
        public float offset_right;
        public float offset_bottom;
    }

    //Spot result structure
    public struct tagSpotValue
    {
        //!< Color values to get
        public short color;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        //!< Color values
        public float[] result;
    }

    //Date structure
    public struct tagDate
    {
        public int Year;
        public int Month;
        public int Day;
    }
    [DllImport("CA2DSDK.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

    //------------------------------------------------CA2DSDK.dll API----------------------------------------------------
    //Enable the SDK
    public static extern int CA2DSDK_Enable();
    [DllImport("CA2DSDK.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    //End usage of the SDK
    public static extern int CA2DSDK_Disable();
    [DllImport("CA2DSDK.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    //Connect to the specified instrument
    public static extern int CA2DSDK_ConnectInstrument(int index);
    [DllImport("CA2DSDK.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    //Disconnect from the connected instrument
    public static extern int CA2DSDK_DisconnectInstrument();
    [DllImport("CA2DSDK.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    //Check the connection state
    public static extern int CA2DSDK_IsConnectInstrument();
    [DllImport("CA2DSDK.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    //Get the number of instruments connected to the PC
    public static extern int CA2DSDK_GetInstrumentCount();
    [DllImport("CA2DSDK.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    //Get the serial number of the specified instrument
    public static extern int CA2DSDK_GetInstrumentSerialNumber(int index);
    [DllImport("CA2DSDK.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    //Set the measurement conditions
    public static extern int CA2DSDK_SetInstrumentCondition(ref tagInstrumentCond pInstCond);
    [DllImport("CA2DSDK.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    //Get the measurement conditions
    public static extern int CA2DSDK_GetInstrumentCondition(ref tagInstrumentCond pInstCond);
    [DllImport("CA2DSDK.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    //Set the user calibration data
    public static extern int CA2DSDK_SetUserCalibrationData(ref tagUserCalData pInstCond);
    [DllImport("CA2DSDK.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    //Get the user calibration data
    public static extern int CA2DSDK_GetUserCalibrationData(ref tagUserCalData pInstCond);
    [DllImport("CA2DSDK.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    //Start the measurement
    public static extern int CA2DSDK_DoMeasurement();
    [DllImport("CA2DSDK.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    //Check the measurement
    public static extern int CA2DSDK_PollingMeasurement();
    [DllImport("CA2DSDK.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    //Stop the completion of the measurement
    public static extern int CA2DSDK_StopMeasurement();
    [DllImport("CA2DSDK.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    //Get the data
    public static extern int CA2DSDK_GetAreaData(ref tagGetDataParam pDataParam, ref float pData);
    [DllImport("CA2DSDK.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    //Set the conditions for calculating data
    public static extern int CA2DSDK_SetDataCondition(ref tagDataCond pDataCond);
    [DllImport("CA2DSDK.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    //Get the data settings
    public static extern int CA2DSDK_GetDataCondition(ref tagDataCond pDataCond);
    [DllImport("CA2DSDK.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    //Clear the evaluation areas
    public static extern int CA2DSDK_ClearEvaluationArea();
    [DllImport("CA2DSDK.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    //Set the evaluation area layout conditions
    public static extern int CA2DSDK_SetEvaluationAreaCondition(ref tagEvaluationCond pCond);
    [DllImport("CA2DSDK.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    //Get the evaluation area layout conditions
    public static extern int CA2DSDK_GetEvaluationAreaCondition(ref tagEvaluationCond pCond);
    [DllImport("CA2DSDK.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    //Add an evaluation area
    public static extern int CA2DSDK_AddEvaluationArea(ref tagEvaluationArea pArea);
    [DllImport("CA2DSDK.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    //Get the number of evaluation areas
    public static extern int CA2DSDK_GetEvaluationAreaCount(ref int pCount);
    [DllImport("CA2DSDK.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    //Get the evaluation area
    public static extern int CA2DSDK_GetEvaluationArea(int num, ref tagEvaluationArea pArea);
    [DllImport("CA2DSDK.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    //Set the spot aligned layout conditions
    public static extern int CA2DSDK_SetAlignedSpotCondition(ref tagAlignedSpotCond pCond);
    [DllImport("CA2DSDK.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    //Get the spot aligned layout conditions
    public static extern int CA2DSDK_GetAlignedSpotCondition(ref tagAlignedSpotCond pCond);
    [DllImport("CA2DSDK.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    //Calculate the spot results
    public static extern int CA2DSDK_CalculateSpotValue();
    [DllImport("CA2DSDK.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    //Get the spot result
    public static extern int CA2DSDK_GetSpotValue(int num, int point, ref tagSpotValue pResult);
    [DllImport("CA2DSDK.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    //Start the instrument diagnostic
    public static extern int CA2DSDK_DiagnosisInstrument();
    [DllImport("CA2DSDK.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    //Check the instrument diagnostic
    public static extern int CA2DSDK_PollingDiagnosis();
    [DllImport("CA2DSDK.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    //Stop the completion of the instrument diagnostic
    public static extern int CA2DSDK_StopDiagnosis();
    [DllImport("CA2DSDK.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    //Get the finder image
    public static extern int CA2DSDK_GetFinderImage(ref short pData);
    [DllImport("CA2DSDK.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    //Check if periodic calibration is required
    public static extern int CA2DSDK_CheckPeriodicalCalibration(ref tagPeriodicalCalInfo pInfo);
    [DllImport("CA2DSDK.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    //Get the SDK version
    public static extern int CA2DSDK_GetSDKVersion();

}
