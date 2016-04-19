using KonikaGlo.colors;
using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using Accord.Imaging.Converters;
using KonikaGlo.spots;
using System.Collections.Generic;
using System.Linq;


namespace KonikaGlo
{
    public partial class MainForm : Form
    {
        // Contains all measurements
        private const int IMAGESIZE = 980;
        private const int COMPACTSIZE = IMAGESIZE - 280;
        private bool active = false;
        private short top = -1, left = -1, right = -1, bottom = -1; 
        private CL.tagEvaluationArea pArea = new CL.tagEvaluationArea();
        private Hashtable hash = new Hashtable();
        public Dictionary<int, float[,]> rawData = new Dictionary<int, float[,]>();
        public Dictionary<String, SortedDictionary<int, Dictionary<String, float>>> results = new Dictionary<String, SortedDictionary<int, Dictionary<String, float>>>();
        public Dictionary<String, float> summaries = new Dictionary<String, float>();
        public Dictionary<String, float> ranges = new Dictionary<String, float>();
        public String testedBy = "";

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            if (!active)
            {
                initInstrument();
                active = true;
            }
        }

        private void screenSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            setInstrumentCondition(128);

            log("------ INSTRUMENT INITIALIZATION COMPLETED -------", 1);
            start.Enabled = true;
        }

        private void setInstrumentCondition(short additional) {
              //Set the measurement conditions
            log("Set instrument condition . . . ", 1);
            CL.tagInstrumentCond cond = instrumentCondition(this.screenSize.Text, additional);
            int ret = CL.CA2DSDK_SetInstrumentCondition(ref cond);
            if (ret < 0)
            {
                log("Set instrument condition", ret);
                return;
            }
            log("Set instrument condition completed", 1);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Disconnect from the connected instrument
            CL.CA2DSDK_DisconnectInstrument();
            //End usage of the SDK
            CL.CA2DSDK_Disable();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            results.Clear();
            logbox.Items.Clear();
            pictureGray.Image = null;
            checkAlignment.Enabled = false;
            start.Enabled = false;
            sendToMes.Enabled = false;
            setInstrumentCondition(1);
            if (!measure()) return;
            if (!getAllData()) return;
            if (!setAutoEvaluationArea()) return;
            if (!processSpotLayouts(true)) return;
            start.Enabled = true;
            sendToMes.Enabled = true;
            checkAlignment.Enabled = true;
        }


        private void start_Click(object sender, EventArgs e)
        {
            if (user.Text == "")
            {
                MessageBox.Show("Select current user.");
                return;
            }

            results.Clear();
            logbox.Items.Clear();
            pictureGray.Image = null;
            start.Enabled = false;
            checkAlignment.Enabled = false;
            sendToMes.Enabled = false;

            setInstrumentCondition(128);
            // Start and pull measurements
            if (!measure()) return;
            // Retrieve all 10 data points and store it in this.hash
            if (!getAllData()) return;
            // Set evaluation area to auto match the display area
            if (!setAutoEvaluationArea()) return;
            // Setup and Calculate spots 
            if (!processSpotLayouts(false)) return;
            // Calculate summary for each spot
            processSummaries();
            // Prepare raw data to upload to MES
            processRawData();

            // Display results in output
#if DEBUG
            /*       foreach (KeyValuePair<String, SortedDictionary<int, Dictionary<String, double>>> meas in results)
                    {
                        foreach (KeyValuePair<int, Dictionary<String, double>> pair in meas.Value)
                        {
                            Console.Write("{0} {1}-> ", meas.Key, pair.Key);
                            foreach (KeyValuePair<String, double> value in pair.Value)
                            {
                                Console.Write("{0}: {1}, ", value.Key, value.Value);
                            }
                            Console.WriteLine("");
                        }
                    } */
#endif

            log("-------  PROCESSING COMPLETED -------", 1);
            start.Enabled = true;
            sendToMes.Enabled = true;
            checkAlignment.Enabled = true;
        }
        private void sendToMes_Click(object sender, EventArgs e)
        {
            SendToMes form = new SendToMes();
            form.screenSize = this.screenSize.Text;
            form.ShowDialog();
        }

        private void processRawData()
        {
            rawData.Clear();
            if (hash.Keys.Count == 0)
                return;

            int border = 8;
            int[] param = new int[4] { 3, 4, 5, 8 };
            for (int k = 0; k < 4; k++)
            {
                rawData.Add(param[k], new float[pArea.right + border - (pArea.left - border), pArea.bottom + border - (pArea.top - border)]);
                int t = 0;
                for (int i = 0; i < IMAGESIZE; i++)
                {
                    for (int j = 0; j < IMAGESIZE; j++)
                    {
                        if (j >= pArea.left - border && i >= pArea.top - border && j < pArea.right + border && i < pArea.bottom + border)
                        {
                            float val = ((float[])hash[param[k]])[t];
                            rawData[param[k]][j - (pArea.left - border), i - (pArea.top - border)] = val;  
                        }
                        t++;
                    }
                }
            }
        }

        private void processSummaries()
        {
            summaries.Clear();
            foreach (KeyValuePair<string, List<Spot>> pair in Definitions.instance("").Measures)
            {
                summaries = summaries.Concat(Summary.calculate(pair.Key, results[pair.Key])).ToDictionary(e => e.Key, e => e.Value);
            }

            unif13.Text = summaries["unif_13pt"].ToString("F2") + "%";
            avg13.Text = summaries["avg_13pt"].ToString("F2");
            ciex13.Text = summaries["ciex_13pt"].ToString("F4");
            ciey13.Text = summaries["ciey_13pt"].ToString("F4");
            arb13.Text = summaries["color_shift_arbitrary_13pt"].ToString("F5");
            adj13.Text = summaries["color_shift_adjacent_13pt"].ToString("F5");
            pur13.Text = summaries["pur_13pt"].ToString("F4");


            unif50.Text = summaries["unif_50pt"].ToString("F2") + "%";
            avg50.Text = summaries["avg_50pt"].ToString("F2");
            ciex50.Text = summaries["ciex_50pt"].ToString("F4");
            ciey50.Text = summaries["ciey_50pt"].ToString("F4");
            arb50.Text = summaries["color_shift_arbitrary_50pt"].ToString("F5");
            adj50.Text = summaries["color_shift_adjacent_50pt"].ToString("F5");
            pur50.Text = summaries["pur_50pt"].ToString("F4");

            unif69.Text = summaries["unif_69pt"].ToString("F2") + "%";
            avg69.Text = summaries["avg_69pt"].ToString("F2");
            ciex69.Text = summaries["ciex_69pt"].ToString("F4");
            ciey69.Text = summaries["ciey_69pt"].ToString("F4");
            arb69.Text = summaries["color_shift_arbitrary_69pt"].ToString("F5");
            adj69.Text = summaries["color_shift_adjacent_69pt"].ToString("F5");
            pur69.Text = summaries["pur_69pt"].ToString("F4");

            unif135.Text = summaries["unif_135pt"].ToString("F2") + "%";
            avg135.Text = summaries["avg_135pt"].ToString("F2");
            ciex135.Text = summaries["ciex_135pt"].ToString("F4");
            ciey135.Text = summaries["ciey_135pt"].ToString("F4");
            arb135.Text = summaries["color_shift_arbitrary_135pt"].ToString("F5");
            adj135.Text = summaries["color_shift_adjacent_135pt"].ToString("F5");
            pur135.Text = summaries["pur_135pt"].ToString("F4");

            unif805.Text = summaries["unif_805pt"].ToString("F2") + "%";
            avg805.Text = summaries["avg_805pt"].ToString("F2");
            ciex805.Text = summaries["ciex_805pt"].ToString("F4");
            ciey805.Text = summaries["ciey_805pt"].ToString("F4");
            arb805.Text = summaries["color_shift_arbitrary_805pt"].ToString("F5");
            adj805.Text = summaries["color_shift_adjacent_805pt"].ToString("F5");
            pur805.Text = summaries["pur_805pt"].ToString("F4");

            unif807.Text = summaries["unif_807pt"].ToString("F2") + "%";
            avg807.Text = summaries["avg_807pt"].ToString("F2");
            ciex807.Text = summaries["ciex_807pt"].ToString("F4");
            ciey807.Text = summaries["ciey_807pt"].ToString("F4");
            arb807.Text = summaries["color_shift_arbitrary_807pt"].ToString("F5");
            adj807.Text = summaries["color_shift_adjacent_807pt"].ToString("F5");
            pur807.Text = summaries["pur_807pt"].ToString("F4");
        }
      
        private Boolean processSpotLayouts(Boolean isOne)
        {
            int ret = 0;
            if (this.top > 0 && this.left > 0 && this.right > 0 && this.bottom > 0)
            {
                CL.tagEvaluationArea pMan = new CL.tagEvaluationArea();
                pMan.top = this.top;
                pMan.left = this.left;
                pMan.right = this.right;
                pMan.bottom = this.bottom;
                ret = CL.CA2DSDK_AddEvaluationArea(ref pMan);
                if (ret < 0)
                {
                    log("Add evaluation area", ret);
                }
            }

            foreach (KeyValuePair<string, List<Spot>> pair in Definitions.instance(screenSize.Text).Measures)
            {
                if (!processSpots(pair.Key, pair.Value)) return false;

                if (isOne)
                    return true;
            }
            return true;
        }

        private Boolean processSpots(String spotName, List<Spot> spotsLayout) {

            int ret = 0;
            // Dictionary for output. Key represents spot index, value is map of variable names and values
            SortedDictionary<int, Dictionary<string, float>> output = new SortedDictionary<int, Dictionary<string, float>>();
 
            foreach (Spot spotLayout in spotsLayout)
            {
                ret = CL.CA2DSDK_SetAlignedSpotCondition(ref spotLayout._cond);
                if ((ret < 0))
                {
                    log("Set aligned spot conditions for " + spotName, ret);
                    return false;
                }
                log("Set aligned spot conditions completed for " + spotName, 1);

                ret = CL.CA2DSDK_GetEvaluationArea(0, ref pArea);
                if (ret < 0)
                {
                    log("Get evaluation area", ret);
                    return false;
                }
                log("Evaluation area T: " + pArea.top.ToString() + " L: " + pArea.left.ToString() + " B: " + pArea.bottom.ToString() + " R: " + pArea.right.ToString(), 1);

                //Calculate the spot results
                ret = CL.CA2DSDK_CalculateSpotValue();
                if ((ret < 0))
                {
                    log("Calculate spot value for " + spotName, ret);
                    return false;
                }
                log("Calculate spot value completed for " + spotName, 1);
                            

                // Draw gray area with crosshairs to check alignment
                Draw(pictureGray, -1);
                // Draw colors for Lv, x, y and tcp
                Draw(picLv, 3);
                Draw(picx, 4);
                Draw(picy, 5);
                Draw(picTcp, 8);

                //Get the spot results for 0 -> X Y Z, 1 -> Lv x y, 2 -> Y u' v', 3 -> tcp duv, 5 -> dw pur 
                for (short color = 0; color <= 5; color++)
                {
                    if (color == 4) {
                        continue;
                    }
                    CL.tagSpotValue spot_val = new CL.tagSpotValue();
                    spot_val.color = color;
                    spot_val.result = new float[3];
                    for (short i = 0; i < spotLayout._cond.row * spotLayout._cond.col; i++)
                    {
                        ret = CL.CA2DSDK_GetSpotValue((short)0, (short)i, ref spot_val);
                        if ((ret < 0))
                        {
                            log("Retrieve spot value " + spotName + ", " + color.ToString(), ret);
                            return false;
                        }

                        // Remove over and under values
                        for (int z = 0; z <= 2; z++)
                        {
                            spot_val.result[z] = pixelUO(spot_val.result[z], -1);
                        }

                        // Create entry in map if not existent
                        if (!output.ContainsKey(spotLayout._order[i]))
                        {
                            output.Add(spotLayout._order[i], new Dictionary<string, float>());
                        }
                        switch (color)
                        {
                            case 0:
                                output[spotLayout._order[i]].Add("X", spot_val.result[0]);
                                output[spotLayout._order[i]].Add("Y", spot_val.result[1]);
                                output[spotLayout._order[i]].Add("Z", spot_val.result[2]);
                                break;
                            case 1:
                                output[spotLayout._order[i]].Add("Lv", spot_val.result[0]);
                                output[spotLayout._order[i]].Add("x", spot_val.result[1]);
                                output[spotLayout._order[i]].Add("y", spot_val.result[2]);
                                break;
                            case 2:
                                output[spotLayout._order[i]].Add("u", spot_val.result[1]);
                                output[spotLayout._order[i]].Add("v", spot_val.result[2]);
                                break;
                            case 3:
                                output[spotLayout._order[i]].Add("tcp", spot_val.result[1]);
                                output[spotLayout._order[i]].Add("duv", spot_val.result[2]);
                                break;
                            case 5:
                                output[spotLayout._order[i]].Add("dw", spot_val.result[1]);
                                output[spotLayout._order[i]].Add("pur", spot_val.result[2]);
                                break;
                        }
                    }
                }
            }
            
            results.Add(spotName, output);

            return true;
        }

        private Boolean initInstrument()
        {
            int ret = 0;
            //Enable the SDK
            log("Initializing SDK and instrument . . . ", 1);
            ret = CL.CA2DSDK_Enable();
            if ((ret < CL.CA2D_OK))
            {
                log("Initializing SDK and instrument", ret);
                return false;
            }
            log("Initialization of the SDK completed", 1);

            //Connect to the specified instrument
            log("Connecting to the instrument . . . ", 1);
            ret = CL.CA2DSDK_ConnectInstrument(0);
            if ((ret < CL.CA2D_OK))
            {
                log("Connection to the instrument", ret);
                return false;
            }
            log("Connection to the instrument completed", 1);

            return true;
        }

        private Boolean measure() {

            int ret = 0;
            //Start the measurement
            log("Start measurements . . . ", 1);
            ret = CL.CA2DSDK_DoMeasurement();
            if ((ret < 0))
            {
                log("Start measurement ", ret);
                return false;
            }
            while (true)
            {
                ret = CL.CA2DSDK_PollingMeasurement();
                if ((ret == CL.CA2D_OK))
                {
                    //When measurement is complete
                    log("Poll measurement completed", 1);
                    break; // TODO: might not be correct. Was : Exit While
                }
                else if ((ret >= CL.CA2D_OK))
                {
                    log("Poll measurement in progress . . . ", 1);
                }
                else
                {
                    //When an error has occurred
                    log("Poll measurement", ret);
                    return false;
                }
                //wait 2 secs
                System.Threading.Thread.Sleep(2000);
            }
            return true;
        }

        private Boolean getAllData()
        {
            int ret = 0;
            // Get image data
            log("Start getting image data . . . ", 1);
            CL.tagDataCond cond_d = tagDataCondition();
            CL.tagGetDataParam paramArea = tagGetParam();
            hash.Clear();
            ranges.Clear();

            for (int j = CL.VALTYPE_X; j <= CL.VALTYPE_PURITY; j++)
            {
                if (j ==CL. VALTYPE_TCP_JIS || j == CL.VALTYPE_DUV_JIS)
                {
                    continue;
                }
                float[] pData = new float[CL.MAXDATAROW * CL.MAXDATACOL];

                //Set the conditions to calculate data
                cond_d.valueType = (short)j;
                ret = CL.CA2DSDK_SetDataCondition(ref cond_d);
                if ((ret < 0))
                {
                    log("Set data condition for " + j.ToString(), ret);
                    return false;
                }

                //Get the data for the specified area
                ret = CL.CA2DSDK_GetAreaData(ref paramArea, pData);
                if ((ret < 0))
                {
                    log("Get area data for " + j.ToString(), ret);
                    return false;
                }

                // Remove and flag over and under error pixels
                for (int z = 0; z < CL.MAXDATACOL * CL.MAXDATAROW; z++)
                {
                    pData[z] = pixelUO(pData[z], j);
                }
                hash.Add(j, pData);
            }
            log("Get image data completed", 1);
            return true;
        }

        private Boolean setAutoEvaluationArea ()
        {
            int ret = 0;
            //Clear the evaluation areas
            ret = CL.CA2DSDK_ClearEvaluationArea();
            if (ret < 0)
            {
                log("Clear evaluation area", ret);
                return false;
            }
            log("Clear evaluation area completed", 1);

            //Set the evaluation area layout conditions
            short t = 1;
            if (this.top > 0 && this.left > 0 && this.right > 0 && this.bottom > 0)
            {
                t = 0;
            }
            CL.tagEvaluationCond cond_e = tagEvaluationCond(t);
            ret = CL.CA2DSDK_SetEvaluationAreaCondition(ref cond_e);
            if (ret < 0)
            {
                log("Set evaluation area condition", ret);
                return false;
            }
                    
            log("Set evaluation area condition completed", 1);
            return true;
        }

        private float pixelUO (float val, int param)
        {
            if (val < -3.0E+38)
            {
                // Over error pixel
                return -1;
            }
            else if ((-3.0E+38 <= val) && (val < -2.0E+38))
            {
                // Under error pixel
               return -1;
            }
            else if (val < -1.0E+38)
            {
                // Calculation error pixel
                return -1;
            }
            else
            {
                if (param != -1)
                {
                    if (!ranges.ContainsKey(param.ToString() + "x"))
                    {
                        ranges.Add(param.ToString() + "x", -1);
                    }
                    if (!ranges.ContainsKey(param.ToString() + "n"))
                    {
                        ranges.Add(param.ToString() + "n", 10000000);
                    }
                    if (val > ranges[param.ToString() + "x"])
                    {
                        ranges[param.ToString() + "x"] = val;
                    }
                    if (val < ranges[param.ToString() + "n"])
                    {
                        ranges[param.ToString() + "n"] = val;
                    }
                }
                return val;
            }
        }

        private void Draw(PictureBox picture, int param)
        {
            ArrayToImage imageConverter = new ArrayToImage(COMPACTSIZE, COMPACTSIZE);
            if (hash.Keys.Count == 0)
                return;
            Bitmap bitmap = new Bitmap(COMPACTSIZE, COMPACTSIZE);
            Color[] d = new Color[COMPACTSIZE * COMPACTSIZE];
            
            int z = 0;
            int t = 0;
            for (int i = 0; i < IMAGESIZE; i++)
            {
                for (int j = 0; j < IMAGESIZE; j++)
                {
                    if (i >= 140 && j >= 140 && i < 840 && j < 840)
                    {
                        float Lv = ((float[])hash[3])[t];
                        float u = ((float[])hash[6])[t];
                        float v = ((float[])hash[7])[t];
                        if (Lv >= 0 && u >= 0 && v >= 0)
                        {
                            if (d[z] != Color.White)
                            {
                                if (param == -1)
                                    d[z] = Color.DarkGray;
                                else
                                {
                                    float val = ((float[])hash[param])[t];
                                    float min = ranges[param.ToString() + "n"];
                                    float max = ranges[param.ToString() + "x"];
                                    d[z] = InterpolateColor((val - min) / (max - min));
                                }
                            }
                        }
                        else
                        {
                            if (d[z] != Color.White)
                                d[z] = Color.Black;
                        }

                        // Mark the corners if this is just gray image
                        if (param == -1)
                        {
                            int x1 = t % IMAGESIZE - pArea.left;
                            int x2 = t % IMAGESIZE - pArea.right;
                            int y1 = (int)(t / IMAGESIZE) - pArea.top;
                            int y2 = (int)(t / IMAGESIZE) - pArea.bottom;
                            if ((x1 == 0 && y1 == 0) || (x1 == 0 && y2 == 0) || (x2 == 0 && y1 == 0) || (x2 == 0 && y2 == 0))
                            {
                                for (int g = -20; g < 20; g++)
                                {
                                        if (z + g >= 0 && z + g < d.Length)
                                            d[z + g] = Color.White;
                                        if (z - COMPACTSIZE + g >= 0 && z - COMPACTSIZE + g < d.Length)
                                            d[z - COMPACTSIZE + g] = Color.White;
                                        if (z + g * COMPACTSIZE >= 0 && z + g * COMPACTSIZE < d.Length)
                                            d[z + g * COMPACTSIZE] = Color.White;
                                        if (z - 1 + g * COMPACTSIZE >= 0 && z - 1 + g * COMPACTSIZE < d.Length)
                                            d[z - 1 + g * COMPACTSIZE] = Color.White;
                                }
                            }
                        }

                        if (param == 3 && ((this.top > 0 && this.left > 0) || (this.right > 0 && this.bottom > 0)))
                        {
                            int x1 = t % IMAGESIZE - this.left;
                            int x2 = t % IMAGESIZE - this.right;
                            int y1 = (int)(t / IMAGESIZE) - this.top;
                            int y2 = (int)(t / IMAGESIZE) - this.bottom;
                            if ((x1 == 0 && y1 == 0) || (x1 == 0 && y2 == 0) || (x2 == 0 && y1 == 0) || (x2 == 0 && y2 == 0))
                            {
                                for (int g = -200; g <200; g++)
                                {
                                    if (z + g >= 0 && z + g < d.Length)
                                        d[z + g] = Color.White;
                                    if (z - COMPACTSIZE + g >= 0 && z - COMPACTSIZE + g < d.Length)
                                        d[z - COMPACTSIZE + g] = Color.White;
                                    if (z + g * COMPACTSIZE >= 0 && z + g * COMPACTSIZE < d.Length)
                                        d[z + g * COMPACTSIZE] = Color.White;
                                    if (z - 1 + g * COMPACTSIZE >= 0 && z - 1 + g * COMPACTSIZE < d.Length)
                                        d[z - 1 + g * COMPACTSIZE] = Color.White;
                                }
                            }
                        }
                        z++;
                    }
                    t++;
                }
            }
            imageConverter.Convert(d, out bitmap);
            picture.Image = bitmap;
        }

        private Color InterpolateColor(float x)
        {
            Color[] colors = new Color[3];
            colors[0] = Color.Blue;
            colors[1] = Color.Green;
            colors[2] = Color.Red;

            double r = 0.0, g = 0.0, b = 0.0;
            double total = 0.0;
            double step = 1.0 / (double)(colors.Length - 1);
            double mu = 0.0;
            double sigma_2 = 0.035;

            foreach (Color color in colors)
            {
                total += Math.Exp(-(x - mu) * (x - mu) / (2.0 * sigma_2)) / Math.Sqrt(2.0 * Math.PI * sigma_2);
                mu += step;
            }

            mu = 0.0;
            foreach (Color color in colors)
            {
                double percent = Math.Exp(-(x - mu) * (x - mu) / (2.0 * sigma_2)) / Math.Sqrt(2.0 * Math.PI * sigma_2);
                mu += step;

                r += color.R * percent / total;
                g += color.G * percent / total;
                b += color.B * percent / total;
            }

            return Color.FromArgb(255, (byte)r, (byte)g, (byte)b );
        }

        private short getExposureMode()
        {
            if (exposure.Text == "AUTO")
                return CL.EXPOSURE_AUTO;
            else
                return CL.EXPOSURE_MANUAL;
        }

        private short getExposureIndex()
        {
            switch (exposure.Text)
	        {
                case "1/2048  1.5%":
                    return 0;
                case "1/1024  1.5%":
                    return 1;
                 case "1/512  1.5%":
                    return 2;
                 case "1/256  1.5%":
                    return 3;
                 case "1/128  1.5%":
                    return 4;
                 case "1/64  1.5%":
                    return 5;
                 case "1/64  3.0%":
                    return 6;
                 case "1/64  6.0%":
                    return 7;
                 case "1/64  12.5%":
                    return 8;
                 case "1/64  25.0%":
                    return 9;
                 case "1/64  50.0%":
                    return 10;
                 case "1/64  100.0%":
                    return 11;
                 case "1/32  100.0%":
                    return 12;
                 case "1/16  100.0%":
                    return 13;
                 case "1/8  100.0%":
                    return 14;
                 case "1/4  100.0%":
                    return 15;
                 case "1/2  100%":
                    return 16;
                 case "1/1  100%":
                    return 17;
                default:
                    return 0;
            }
        }

        private CL.tagInstrumentCond instrumentCondition(String screenSize, short additional)
        {
            CL.tagInstrumentCond cond = default(CL.tagInstrumentCond);


            cond.exposureMode = getExposureMode();
            cond.exposureIndex = getExposureIndex();

            //Normal lens
            cond.lensType = CL.LENS_NORMAL;

                     
            cond.lensPosition = 4;
            //0.5m
            cond.measurementType = CL.SYNCMODE_OFF;
            //Synchronized measurements : Don't care
            cond.syncValue = 5.0;
            cond.additional = additional;
            cond.left = 0;
            cond.top = 0;
            cond.right = IMAGESIZE - 1;
            cond.bottom = IMAGESIZE - 1;
            cond.filterMeasure = CL.FILTER_MEASURE_OFF;
            //XYZ measurements
            cond.filterIndex = CL.FILTER_INDEX_Y;
            cond.smearIndex = CL.SMEAR_NONE;
            cond.userCal = CL.USERCAL_OFF;
            cond.rotate = CL.ROTATION_NONE;
            return cond;
        }


        //The conditions to calculate data
        private CL.tagDataCond tagDataCondition()
        {
            CL.tagDataCond cond_d = default(CL.tagDataCond);
            cond_d.lower_enable = 1;
            cond_d.lower_item = 1;
            cond_d.lower_threshold = 3;
            cond_d.resolution = 0;
            return cond_d;
        }

        //Data acquisition area
        private CL.tagGetDataParam tagGetParam()
        {
            CL.tagGetDataParam paramArea = default(CL.tagGetDataParam);
            paramArea.left = 0;
            paramArea.top = 0;
            paramArea.right = IMAGESIZE - 1;
            paramArea.bottom = IMAGESIZE - 1;
            return paramArea;
        }

        // Evaluation area condition
        private CL.tagEvaluationCond tagEvaluationCond(short t)
        {
            CL.tagEvaluationCond cond_e = default(CL.tagEvaluationCond);
            cond_e.type = t;
            cond_e.left = 5;
            cond_e.top = 5;
            cond_e.right = IMAGESIZE - 5;
            cond_e.bottom = IMAGESIZE - 5;
            cond_e.row = 1;
            cond_e.col = 1;
            cond_e.thresholdValue = (double)threshold.Value;
            return cond_e;
        }

        private void picLv_Click(object sender, EventArgs e)
        {
            MouseEventArgs mea = (MouseEventArgs)e;
            if (mea.Button == MouseButtons.Left)
            {
                this.left = (short)(mea.X + 140);
                this.top = (short)(mea.Y + 140);
            }
            if (mea.Button == MouseButtons.Right)
            {
                this.right = (short)(mea.X + 140);
                this.bottom = (short)(mea.Y + 140);
            }
            Draw(picLv, 3);
        }

        private void log(String text, int error)
        {
            if (error < 1)
            {
                start.Enabled = true;
                text = "ERROR!!! -> " + text + ": " + error.ToString();
            }
            logbox.Items.Add(text);
            Application.DoEvents();
        }

      
        private void button1_Click_1(object sender, EventArgs e)
        {
            this.top = -1;
            this.left = -1;
            this.right = -1;
            this.bottom = -1;
            Draw(picLv, 3);
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
