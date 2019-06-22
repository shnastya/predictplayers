using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PredictPlayers
{
    public partial class MainForm : Form
    {
        string resultFile = @"Save\Data.txt";
        string clustersFile = @"AVERAGE.txt";
        string clustersFile_N = @"AVERAGE_N.txt";
        string clustersPath = "Save\\Clusters\\model";
        int countTest = 0;
        int countParams = 5;
        int countFeatures = 14;
        Prognostication prognostication;
        TextBox txtBoxEps = new TextBox();
        TextBox txtBoxMinSamples = new TextBox();
        Label lblEps = new Label();
        Label lblMinSamples = new Label();
        TextBox txtBoxDamping = new TextBox();
        TextBox txtBoxMaxIter = new TextBox();
        TextBox txtBoxConvergIter = new TextBox();
        Label lblDamp = new Label();
        Label lblMaxIter = new Label();
        Label lblConvergIter = new Label();


        public MainForm()
        {
            InitializeComponent();
            //btnClustering.Enabled = false;
            FillAlgorithm();

            listBoxMetrics.Items.Add("Presicion -> 1");
            listBoxMetrics.Items.Add("Recall -> 1");
            listBoxMetrics.Items.Add("f-score -> 1");
            listBoxMetrics.Items.Add("Silhouette -> 1");
            listBoxMetrics.Items.Add("Q -> 0");
            listBoxMetrics.Items.Add("β -> 1");
            listBoxMetrics.Items.Add("%ошибок -> 0");

            prognostication = new Prognostication();
            FillTableFeatures();
            CreateElements();

            labelAlg.Text = "";
            labelCountClusters.Text = "";
            labelStay.Text = "";
            labelLeave.Text = "";
        }

        private void CreateElements()
        {
            lblEps.Text = "Eps-окрестность";
            lblMinSamples.Text = "Min samples";
            txtBoxEps.Location = new Point(181, 41);
            txtBoxMinSamples.Location = new Point(324, 41);
            txtBoxEps.Size = new Size(136, 25);
            txtBoxMinSamples.Size = new Size(136, 23);
            lblEps.Location = new Point(178, 23);
            lblMinSamples.Location = new Point(324,23);
            lblEps.Size = new Size(140, 15);
            txtBoxEps.TextAlign = HorizontalAlignment.Right;
            txtBoxMinSamples.TextAlign = HorizontalAlignment.Right;

            groupBoxParam.Controls.Add(txtBoxEps);
            groupBoxParam.Controls.Add(txtBoxMinSamples);
            groupBoxParam.Controls.Add(lblEps);
            groupBoxParam.Controls.Add(lblMinSamples);

            txtBoxMinSamples.KeyPress += Int_KeyPress;
            txtBoxEps.KeyPress += Float_KeyPress;

            lblDamp.Text = "Damping";
            lblMaxIter.Text = "Max_iter";
            lblConvergIter.Text = "Convergence_iter";
            lblDamp.Location = new Point(178, 23);
            lblMaxIter.Location = new Point(324, 23);
            lblConvergIter.Location = new Point(465, 23);
            txtBoxDamping.Location = new Point(181, 41);
            txtBoxMaxIter.Location = new Point(324, 41);
            txtBoxConvergIter.Location = new Point(465, 41);
            txtBoxDamping.Size = new Size(136, 25);
            txtBoxMaxIter.Size = new Size(136, 25);
            txtBoxConvergIter.Size = new Size(136, 25);
            lblDamp.Size = new Size(140, 15);
            lblMaxIter.Size = new Size(140, 15);
            lblConvergIter.Size = new Size(140, 15);
            groupBoxParam.Controls.Add(lblDamp);
            groupBoxParam.Controls.Add(lblMaxIter);
            groupBoxParam.Controls.Add(lblConvergIter);
            groupBoxParam.Controls.Add(txtBoxDamping);
            groupBoxParam.Controls.Add(txtBoxMaxIter);
            groupBoxParam.Controls.Add(txtBoxConvergIter);

            txtBoxDamping.KeyPress += Float_KeyPress;
            txtBoxMaxIter.KeyPress += Int_KeyPress;
            txtBoxConvergIter.KeyPress += Int_KeyPress;
        }

        private void Int_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar)))
            {
                if (e.KeyChar != (char)Keys.Back)
                {
                    e.Handled = true;
                }
            }
        }

        private void Float_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar)) && !((e.KeyChar == ',')))
            {
                if (e.KeyChar != (char)Keys.Back)
                {
                    e.Handled = true;
                }
            }
        }

        private void FillTableFeatures()
        {
            int index;

            foreach(string name in prognostication.features)
            {
                index = dataGridFeatures.Rows.Add();
                dataGridFeatures.Rows[index].Cells[0].Value = name;
            }
        }

        private void FillAlgorithm()
        {
            cmbBoxAlg.Items.Add("Hierarchical agglomerative");
            cmbBoxAlg.Items.Add("KMeans");
            cmbBoxAlg.Items.Add("DBSCAN");
            cmbBoxAlg.Items.Add("Affinity Propagation");

            string[] metrics = new string[]
            {
                "euclidean", "l1", "l2", "manhattan", "cosine"
            };
            string[] linkage = new string[]
            {
                "ward", "complete", "average", "single"
            };

            string[] init = new string[]
            {
                "k-means++", "random"
            };

            string[] alg = new string[]
            {
                "auto", "full", "elkan"
            };
            cmbBoxMetric.Items.AddRange(metrics);
            cmbBoxLink.Items.AddRange(linkage);
            cmbBoxInit.Items.AddRange(init);
            cmbBoxAlgMeans.Items.AddRange(alg);
            cmbBoxAlg.SelectedIndex = 0;
            cmbBoxLink.SelectedIndex = 0;
            cmbBoxMetric.SelectedIndex = 0;
            cmbBoxInit.SelectedIndex = 0;
            cmbBoxAlgMeans.SelectedIndex = 0;

            SwitchAlgorithm();
        }

        private void SetVisibleElements(bool H, bool K, bool D, bool A)
        {
            cmbBoxMetric.Visible = H;
            cmbBoxLink.Visible = H;
            lblMetrics.Visible = H;
            lblLink.Visible = H;

            cmbBoxInit.Visible = K;
            cmbBoxAlgMeans.Visible = K;
            lblInit.Visible = K;
            lblAlg.Visible = K;

            txtBoxEps.Visible = D;
            txtBoxMinSamples.Visible = D;
            lblMinSamples.Visible = D;
            lblEps.Visible = D;

            txtBoxDamping.Visible = A;
            txtBoxMaxIter.Visible = A;
            txtBoxConvergIter.Visible = A;
            lblDamp.Visible = A;
            lblMaxIter.Visible = A;
            lblConvergIter.Visible = A;

            if (H || K)
            {
                numericCountCluster.Visible = true;
                lblCountClusters.Visible = true;
            }
            else
            {
                numericCountCluster.Visible = false;
                lblCountClusters.Visible = false;
            }

        }

        private void SwitchAlgorithm()
        {
            switch (cmbBoxAlg.SelectedIndex)
            {
                case 0:
                    SetVisibleElements(true, false, false, false);
                    break;
                case 1:
                    SetVisibleElements(false, true, false, false);
                    break;
                case 2:
                    SetVisibleElements(false, false, true, false);
                    break;
                case 3:
                    SetVisibleElements(false, false, false, true);
                    break;

            }
        }

        private bool CheckFiles(int countAttr, string[] attr, string fileName)
        {
            using(StreamReader sr = new StreamReader(fileName))
            {
                var arr = sr.ReadLine().Split(',');
                if (arr.Length != countAttr)
                    return false;
                for (int i = 0; i < countAttr; i++)
                    if (arr[i] != attr[i])
                        return false;
            }

            return true;
        }

        private void ReadData(string dataName)
        {
            string filePath = "";

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                    filePath = openFileDialog.FileName;
            }

            int countAttr = 0;

            switch (dataName)
            {
                case "Battle":
                    countAttr = 18;
                    string[] battle = new string[18] { "id", "transaction_id", "datetime", "user_id", "hero_id", "status",
                    "duration", "ai_share", "experience", "crystals", "mana", "location_id", "opponent_type",
                        "opponent_user_id", "opponent_hero_id", "bot_template_id", "opponent_race_id", "kind" };
                    if (CheckFiles(countAttr, battle, filePath))
                        txtboxBattle.Text = filePath;
                    else
                        MessageBox.Show("Неверный файл!");
                   break;
                case "Hero":
                    countAttr = 7;
                    string[] hero = new string[7] { "id", "transaction_id", "datetime", "user_id", "hero_id", "race", "name" };
                    if (CheckFiles(countAttr, hero, filePath))
                        txtBoxHero.Text = filePath;
                    else
                        MessageBox.Show("Неверный файл!");
                    break;
                case "Level":
                    countAttr = 6;
                    string[] level = new string[6] {"id", "transaction_id", "datetime", "user_id", "hero_id", "delta" };
                    if (CheckFiles(countAttr, level, filePath))
                        txtBoxLevel.Text = filePath;
                    else
                        MessageBox.Show("Неверный файл!");
                    break;
                case "Payment":
                    countAttr = 8;
                    string[] payment = new string[8] { "crystal_payment_id", "created", "user_id", "total", "reason",
                        "payment_system", "transaction_id", "purchase_token" };
                    if (CheckFiles(countAttr, payment, filePath))
                        txtBoxPayment.Text = filePath;
                    else
                        MessageBox.Show("Неверный файл!");
                    break;
                case "Resource":
                    countAttr = 8;
                    string[] resource = new string[8] { "id", "transaction_id", "datetime", "user_id", "hero_id",
                        "reason", "res_id", "total" };
                    if (CheckFiles(countAttr, resource, filePath))
                        txtBoxResource.Text = filePath;
                    else
                        MessageBox.Show("Неверный файл!");
                    break;
                case "Quest":
                    countAttr = 6;
                    string[] quest = new string[6] { "id", "transaction_id", "datetime", "user_id",
                        "hero_id", "quest_id" };
                    if (CheckFiles(countAttr, quest, filePath))
                        txtBoxQuest.Text = filePath;
                    else
                        MessageBox.Show("Неверный файл!");
                    break;
                case "User":
                    countAttr = 4;
                    string[] user = new string[4] { "id", "user_id", "datetime", "region_id" };
                    if (CheckFiles(countAttr, user, filePath))
                        txtBoxUser.Text = filePath;
                    else
                        MessageBox.Show("Неверный файл!");
                    break;
            }
        }

        private bool AllFileUpdated()
        {
            if (txtboxBattle.Text != "" && txtBoxHero.Text != "" && txtBoxLevel.Text != "" &&
                txtBoxPayment.Text != "" && txtBoxQuest.Text != "" &&
                txtBoxResource.Text != "" && txtBoxUser.Text != "")
                return true;
            else return false;
        }

        private void btnWork_Click(object sender, EventArgs e)
        {
            //if (AllFileUpdated())
           // {
                btnWork.Text = "Идет обработка...";
                btnWork.Enabled = false;
                //var t = Task.Run(() => DataProcessing.Start(resultFile, this));
                Task t = new Task(() => DataProcessing.Start(resultFile, this));
                var awaiter = t.GetAwaiter();

                t.ContinueWith((m) =>
                {
                    btnWork.Enabled = true;
                    btnWork.Text = "Обработать";
                    btnClustering.Enabled = true;
                    progressBar.Value = 0;
                    MessageBox.Show("Обработка завершена.");
                    checkBoxNewData.Enabled = true;
                    checkBoxNewData.Checked = false;
                    labelData.Text = "У вас уже есть обработанные данные.";
                    DataLoadingEnabled(false);
                    btnWork.Enabled = false;
                });

                //awaiter.OnCompleted(() =>
                //{
                //    btnWork.Enabled = true;
                //    btnWork.Text = "Обработать";
                //    btnClustering.Enabled = true;
                //    progressBar.Value = 0;
                //    MessageBox.Show("Обработка завершена.");
                //    checkBoxNewData.Enabled = true;
                //    checkBoxNewData.Checked = false;
                //    labelData.Text = "У вас уже есть обработанные данные.";
                //    DataLoadingEnabled(false);
                //    btnWork.Enabled = false;
                //});

                t.Start();
            //}
            //else
             //   MessageBox.Show("Не все файлы загружены!");
        }

        private void CreateFileParams()
        {
            string file = "Params.txt";
            using(StreamWriter sw = new StreamWriter(file))
            {
                switch (cmbBoxAlg.Text)
                {
                    case "Hierarchical agglomerative":
                        sw.WriteLine("H");
                        sw.WriteLine(numericCountCluster.Text);
                        sw.WriteLine(cmbBoxMetric.Text);
                        sw.Write(cmbBoxLink.Text);
                        break;
                    case "KMeans":
                        sw.WriteLine("K");
                        sw.WriteLine(numericCountCluster.Text);
                        sw.WriteLine(cmbBoxInit.Text);
                        sw.WriteLine(cmbBoxAlgMeans.Text);
                        break;
                    case "DBSCAN":
                        sw.WriteLine("D");
                        sw.WriteLine(txtBoxEps.Text.Replace(',', '.'));
                        sw.WriteLine(txtBoxMinSamples.Text);
                        break;
                    case "Affinity Propagation":
                        sw.WriteLine("A");
                        sw.WriteLine(txtBoxDamping.Text.Replace(',','.'));
                        sw.WriteLine(txtBoxMaxIter.Text);
                        sw.WriteLine(txtBoxConvergIter.Text);
                        break;

                }
            }
        }

        private bool CheckBeforeAlgorithm()
        {
            if (cmbBoxAlg.Text == "" || numericCountCluster.Text == "" || cmbBoxLink.Text == "" ||
                cmbBoxMetric.Text == "")
            {
                MessageBox.Show("Не все поля заполнены!");
                return false;
            }
            return true;
        }

        private double PercentError(int L1, int L0, int metka)
        {
            double result;

            if (metka == 1)
                result = 100.0 * L0 / (L0 + L1);
            else
                result = 100.0*L1 / (L0 + L1);

            return Math.Round(result, 3);
        }

        private void FillTable()
        {
            gridErrors.Rows.Clear();
            gridMatrixErrors.Rows.Clear();
            gridMetrics.Rows.Clear();

            string[] notleave = null;
            string[] leave = null;
            int countRow = 4, numbCluster, countNotLeave, countLeave, metka;
            string fileClustering = "ResultClustering.txt";
            string s = "";
            int i = 0, ind;
            int countCluster = 0;

            using (StreamReader sr = new StreamReader(fileClustering))
            {
                s = sr.ReadLine();
                countCluster = Convert.ToInt32(s);

                
                while (i < countRow * countCluster)
                {
                    s = sr.ReadLine();
                    numbCluster = Convert.ToInt32(s);
                    var arr = sr.ReadLine().Split(':');
                    countNotLeave = Convert.ToInt32(arr[1]);
                    arr = sr.ReadLine().Split(':');
                    countLeave = Convert.ToInt32(arr[1]);
                    arr = sr.ReadLine().Split(':');
                    metka = Convert.ToInt32(arr[1]);
                    ind = gridErrors.Rows.Add();
                    gridErrors.Rows[ind].Cells[0].Value = numbCluster;
                    gridErrors.Rows[ind].Cells[1].Value = countLeave;
                    gridErrors.Rows[ind].Cells[2].Value = countNotLeave;
                    gridErrors.Rows[ind].Cells[3].Value = metka;
                    gridErrors.Rows[ind].Cells[4].Value = PercentError(countLeave, countNotLeave, metka);
                    i+=4;
                }

                i = 0;
                for (i = 0; i < 4; i++)
                {
                    s = sr.ReadLine().Trim();
                    if (i == 2)
                    {
                        while (s.Contains("  ")) { s = s.Replace("  ", " "); }
                        notleave = s.Split(' ');
                    }
                    else if (i == 3)
                    {
                        while (s.Contains("  ")) { s = s.Replace("  ", " "); }
                        leave = s.Split(' ');
                    }
                }

                ind = gridMatrixErrors.Rows.Add();
                gridMatrixErrors.Rows[ind].Cells[0].Value = "Остался";
                gridMatrixErrors.Rows[ind].Cells[1].Value = notleave[1].Replace('.',',');
                gridMatrixErrors.Rows[ind].Cells[2].Value = notleave[2].Replace('.', ',');
                gridMatrixErrors.Rows[ind].Cells[3].Value = notleave[3].Replace('.', ',');
                ind = gridMatrixErrors.Rows.Add();
                gridMatrixErrors.Rows[ind].Cells[0].Value = "Ушел";
                gridMatrixErrors.Rows[ind].Cells[1].Value = leave[1].Replace('.', ',');
                gridMatrixErrors.Rows[ind].Cells[2].Value = leave[2].Replace('.', ',');
                gridMatrixErrors.Rows[ind].Cells[3].Value = leave[3].Replace('.', ',');

                for (int j = 0; j < 5; j++)
                    s = sr.ReadLine();
                ind = gridMetrics.Rows.Add();
                gridMetrics.Rows[ind].Cells[1].Value = Math.Round(Convert.ToDouble(s.Replace('.',',')), 3);
                s = sr.ReadLine();
                gridMetrics.Rows[ind].Cells[2].Value = Math.Round(Convert.ToDouble(s.Replace('.',',')), 3);
                s = sr.ReadLine();
                gridMetrics.Rows[ind].Cells[0].Value = Math.Round(Convert.ToDouble(s.Replace('.', ',')), 3);
            }


        }

        private void TaskPython(Process p)
        {
            p.Start();
            p.WaitForExit();
            btnClustering.Text = "Кластеризовать";
            btnClustering.Enabled = true;
            FillTable();
            SaveTest_();
            SaveAverageValuesOfClusters();
            btnOk.Enabled = true;
        }

        private void btnClustering_Click(object sender, EventArgs e)
        {
            string sourceFile = resultFile;
            string normal;
            if (checkBoxNormal.Checked)
                normal = "1";
            else normal = "0";

            string fileClustering = "ResultClustering.txt";
            CreateFileParams();

            if (CheckBeforeAlgorithm())
            {
                btnClustering.Text = "...";
                btnClustering.Enabled = false;
                Process p = new Process();
                p.StartInfo.FileName = "python.exe";
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.Arguments = "clustering.py " + fileClustering + " Params.txt " + sourceFile + " " + normal;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardError = true;
                //Task t = new Task(() => TaskPython(p));
                //t.ContinueWith((m) =>
                //{
                //    btnClustering.Text = "Кластеризовать";
                //    btnClustering.Enabled = true;
                //    FillTable();
                //    SaveTest_();
                //    SaveAverageValuesOfClusters();
                //    btnOk.Enabled = true;
                //});
                //t.Start();
                //p.Start();

                //StreamReader s = p.StandardOutput;
                //string output = s.ReadToEnd();
                //string[] r = output.Split(new char[] { ' ' });
                //Console.WriteLine(r[0]);

                //p.WaitForExit();
                //btnClustering.Text = "Кластеризовать";
                //btnClustering.Enabled = true;
                //FillTable();
                //SaveTest_();
                //SaveAverageValuesOfClusters();
                //btnOk.Enabled = true;


                p.Start();
                p.WaitForExit();
                btnClustering.Text = "Кластеризовать";
                btnClustering.Enabled = true;
                FillTable();
                SaveTest_();
                SaveAverageValuesOfClusters();
                SaveLabels();
                btnOk.Enabled = true;
            }
        }

        void SaveAverageValuesOfClusters()
        {
            string newFile = clustersPath + countTest + ".txt";
            StreamReader sr = new StreamReader(clustersFile);
            StreamWriter sw = new StreamWriter(newFile);
            sw.Write(sr.ReadToEnd());
            sr.Close();
            sw.Close();

            newFile = clustersPath + countTest + "_n.txt";
            sr = new StreamReader(clustersFile_N);
            sw = new StreamWriter(newFile);
            sw.Write(sr.ReadToEnd());
            sr.Close();
            sw.Close();
        }

        void SaveLabels()
        {
            string newFile = "Save\\Json\\labels" + countTest + ".txt";
            StreamReader sr = new StreamReader("Players.txt");
            StreamWriter sw = new StreamWriter(newFile);
            sw.Write(sr.ReadToEnd());
            sr.Close();
            sw.Close();
        }

        //private void AddTest()
        //{
        //    int indexTest = countTest;
        //    using(StreamReader sr = new StreamReader(@"Save\Test\test"+indexTest+".txt"))
        //    {
        //        var arr = sr.ReadLine().Split(' ');
        //        int indRow = gridResult.Rows.Add();
        //        for (int i = 0; i < arr.Length-1; i++)
        //        {
        //            gridResult.Rows[indRow].Cells[i].Value = arr[i];
        //        }
        //    }
        //}

        private void LoadTests()
        {

        }

        private void SaveTest_()
        {
            countTest++;
            StoredResult sr = new StoredResult(gridErrors.Rows.Count);
            sr.silhouette = Convert.ToDouble(gridMetrics.Rows[0].Cells[0].Value);
            sr.Qmetrics = Convert.ToDouble(gridMetrics.Rows[0].Cells[1].Value);
            sr.Bmetrics = Convert.ToDouble(gridMetrics.Rows[0].Cells[2].Value);
            sr.algorithm = cmbBoxAlg.Text;
            sr.stayedPlayers[0] = Convert.ToDouble(gridMatrixErrors.Rows[0].Cells[1].Value);
            sr.stayedPlayers[1] = Convert.ToDouble(gridMatrixErrors.Rows[0].Cells[2].Value);
            sr.stayedPlayers[2] = Convert.ToDouble(gridMatrixErrors.Rows[0].Cells[3].Value);
            sr.leavePlayers[0] = Convert.ToDouble(gridMatrixErrors.Rows[1].Cells[1].Value);
            sr.leavePlayers[1] = Convert.ToDouble(gridMatrixErrors.Rows[1].Cells[2].Value);
            sr.leavePlayers[2] = Convert.ToDouble(gridMatrixErrors.Rows[1].Cells[3].Value);
            for(int i = 0; i < gridErrors.Rows.Count; i++)
            {
                sr.AddCluster(Convert.ToInt32(gridErrors.Rows[i].Cells[0].Value),
                    Convert.ToInt32(gridErrors.Rows[i].Cells[1].Value),
                    Convert.ToInt32(gridErrors.Rows[i].Cells[2].Value),
                    Convert.ToInt32(gridErrors.Rows[i].Cells[3].Value),
                    Convert.ToDouble(gridErrors.Rows[i].Cells[4].Value));
            }
            sr.normalization = checkBoxNormal.Checked;
            switch (cmbBoxAlg.Text)
            {
                case "Hierarchical agglomerative":
                    sr.metrcis = cmbBoxMetric.Text;
                    sr.link = cmbBoxLink.Text;
                    break;
                case "KMeans":
                    sr.init = cmbBoxInit.Text;
                    sr.alg = cmbBoxAlgMeans.Text;
                    break;
                case "Affinity Propagation":
                    sr.damping = Convert.ToDouble(txtBoxDamping.Text);
                    sr.maxIter = Convert.ToInt32(txtBoxMaxIter.Text);
                    sr.convergIter = Convert.ToInt32(txtBoxConvergIter.Text);
                    break;
                case "DBSCAN":
                    sr.eps = Convert.ToDouble(txtBoxEps.Text);
                    sr.minSample = Convert.ToInt32(txtBoxMinSamples.Text);
                    break;
            }
            DataStore.Serialize(countTest, sr);
            int indRow = gridResult.Rows.Add();
            gridResult.Rows[indRow].Cells[0].Value = countTest;
            gridResult.Rows[indRow].Cells[1].Value = sr.silhouette;
            gridResult.Rows[indRow].Cells[2].Value = sr.Qmetrics;
            gridResult.Rows[indRow].Cells[3].Value = sr.Bmetrics;
            gridResult.Rows[indRow].Cells[4].Value = sr.stayedPlayers[2];
            gridResult.Rows[indRow].Cells[5].Value = sr.leavePlayers[2];

            comboBoxNumberTest.Items.Add(countTest);
            comboBoxNumberTest.SelectedIndex = comboBoxNumberTest.Items.Count - 1;
            cmbBoxJsonTest.Items.Add(countTest);
            cmbBoxJsonTest.SelectedIndex = cmbBoxJsonTest.Items.Count - 1;
        }

        private void SaveTest()
        {
            countTest++;
            using(StreamWriter sw = new StreamWriter("Save//Test//test" + countTest.ToString() + ".txt"))
            {
                sw.WriteLine(countTest + " " + gridMetrics.Rows[0].Cells[0].Value.ToString().Replace(',' ,'.') + " " +
                   gridMetrics.Rows[0].Cells[1].Value.ToString().Replace(',', '.') + " " +
                   gridMetrics.Rows[0].Cells[2].Value.ToString().Replace(',', '.') + " " +
                   gridMatrixErrors.Rows[0].Cells[3].Value.ToString() + " " +
                   gridMatrixErrors.Rows[1].Cells[3].Value.ToString() + " ");

                sw.WriteLine("Алгоритм " + cmbBoxAlg.Text);
                sw.WriteLine("Количество_кластеров " + numericCountCluster.Value.ToString());
                sw.WriteLine("Метрика " + cmbBoxMetric.Text);
                sw.WriteLine("Связь " + cmbBoxLink.Text);
                sw.Write("Нормализация ");
                if (checkBoxNormal.Checked)
                    sw.WriteLine("да");
                else sw.WriteLine("нет");
                sw.WriteLine(gridMetrics.Rows[0].Cells[0].Value.ToString() + " " +
                   gridMetrics.Rows[0].Cells[1].Value.ToString() + " " +
                   gridMetrics.Rows[0].Cells[2].Value.ToString());
                sw.WriteLine(gridMatrixErrors.Rows[0].Cells[0].Value.ToString() + " " +
                    gridMatrixErrors.Rows[0].Cells[1].Value.ToString() + " " +
                    gridMatrixErrors.Rows[0].Cells[2].Value.ToString() + " " +
                    gridMatrixErrors.Rows[0].Cells[3].Value.ToString());
                sw.WriteLine(gridMatrixErrors.Rows[1].Cells[0].Value.ToString() + " " +
                    gridMatrixErrors.Rows[1].Cells[1].Value.ToString() + " " +
                    gridMatrixErrors.Rows[1].Cells[2].Value.ToString() + " " +
                    gridMatrixErrors.Rows[1].Cells[3].Value.ToString());
                sw.WriteLine(gridErrors.Rows.Count);
                for (int i = 0; i < gridErrors.Rows.Count; i++)
                    sw.WriteLine(gridErrors.Rows[i].Cells[0].Value.ToString() + " " +
                        gridErrors.Rows[i].Cells[1].Value.ToString() + " " +
                        gridErrors.Rows[i].Cells[2].Value.ToString() + " " +
                        gridErrors.Rows[i].Cells[3].Value.ToString() + " " +
                        gridErrors.Rows[i].Cells[4].Value.ToString());
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnBattle_Click(object sender, EventArgs e)
        {
            ReadData("Battle");
        }

        private void btnHero_Click(object sender, EventArgs e)
        {
            ReadData("Hero");
        }

        private void btnLevel_Click(object sender, EventArgs e)
        {
            ReadData("Level");
        }

        private void btnPayment_Click(object sender, EventArgs e)
        {
            ReadData("Payment");
        }

        private void btnResource_Click(object sender, EventArgs e)
        {
            ReadData("Resource");
        }

        private void btnQuest_Click(object sender, EventArgs e)
        {
            ReadData("Quest");
        }

        private void btnUser_Click(object sender, EventArgs e)
        {
            ReadData("User");
        }

        private void btnClose1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnClose2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cmbBoxLink_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBoxLink.Text == "ward")
            {
                cmbBoxMetric.SelectedIndex = 0;
                cmbBoxMetric.Enabled = false;
            }
            else
            {
                cmbBoxMetric.Enabled = true;
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            countTest = new DirectoryInfo(@"Save\Test").GetFiles().Length;
            if (File.Exists(@"Save\Data.txt"))
            {
                labelData.Text = "У вас уже есть обработанные данные.";
                DataLoadingEnabled(false);
                btnWork.Enabled = false;
                for(int i = 1; i <= countTest; i++)
                {
                    StoredResult sr = DataStore.Deserialize(i);
                    int indRow = gridResult.Rows.Add();
                    gridResult.Rows[indRow].Cells[0].Value = i;
                    gridResult.Rows[indRow].Cells[1].Value = sr.silhouette;
                    gridResult.Rows[indRow].Cells[2].Value = sr.Qmetrics;
                    gridResult.Rows[indRow].Cells[3].Value = sr.Bmetrics;
                    gridResult.Rows[indRow].Cells[4].Value = sr.stayedPlayers[2];
                    gridResult.Rows[indRow].Cells[5].Value = sr.leavePlayers[2];

                    comboBoxNumberTest.Items.Add(i);
                    cmbBoxJsonTest.Items.Add(i);
                }

                if (countTest >= 1)
                {
                    comboBoxNumberTest.SelectedIndex = 0;
                    cmbBoxJsonTest.SelectedIndex = 0;
                }
                else btnOk.Enabled = false;

            }
            else
            {
                labelData.Text = "У вас еще нет обработанных данных.";
                checkBoxNewData.Enabled = false;
            }

            btnPredict.Enabled = false;
        }

        private void gridResult_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int row = gridResult.CurrentRow.Index + 1;
            //string path = @"Save\Test\test" + row.ToString() + ".xml";
            ResultForm f = new ResultForm(row);
            f.Show();
        }

        private void DataLoadingEnabled(bool val)
        {
            txtboxBattle.Enabled = val;
            txtBoxHero.Enabled = val;
            txtBoxLevel.Enabled = val;
            txtBoxPayment.Enabled = val;
            txtBoxQuest.Enabled = val;
            txtBoxResource.Enabled = val;
            txtBoxUser.Enabled = val;
            btnBattle.Enabled = val;
            btnHero.Enabled = val;
            btnLevel.Enabled = val;
            btnPayment.Enabled = val;
            btnQuest.Enabled = val;
            btnResource.Enabled = val;
            btnUser.Enabled = val;
        }

        private void checkBoxNewData_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBoxNewData.Checked)
            {
                DialogResult dr = MessageBox.Show("Вы действительно хотите обработать новые данные? Старые данные будут удалены.",
                    "Предупреждение", MessageBoxButtons.YesNo);
                if(dr == DialogResult.Yes)
                {
                    checkBoxNewData.Enabled = false;
                    DataLoadingEnabled(true);
                    btnWork.Enabled = true;
                    DirectoryInfo dir = new DirectoryInfo(@"Save\Test");
                    foreach (FileInfo file in dir.GetFiles())
                    {
                        file.Delete();
                    }
                    dir = new DirectoryInfo(@"Save\Clusters");
                    foreach (FileInfo file in dir.GetFiles())
                    {
                        file.Delete();
                    }
                    FileInfo fi = new FileInfo(@"Save\Data.txt");
                    fi.Delete();
                    gridResult.Rows.Clear();
                    comboBoxNumberTest.Items.Clear();
                    labelData.Text = "У вас еще нет обработанных данных";
                    return;
                }
                checkBoxNewData.Checked = false;
            }
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void cmbBoxAlg_SelectedIndexChanged(object sender, EventArgs e)
        {
            SwitchAlgorithm();
        }

        private void groupBox12_Enter(object sender, EventArgs e)
        {

        }

        private void btnPredict_Click(object sender, EventArgs e)
        {
            double[] user = new double[countFeatures];
            for(int i = 0; i < countFeatures; i++)
                if (dataGridFeatures.Rows[i].Cells[1].Value == null)
                {
                    MessageBox.Show("Указаны не все значения признаков нового игрока!");
                    return;
                }
                else
                    user[i] = Convert.ToDouble(dataGridFeatures.Rows[i].Cells[1].Value.ToString());

            txtBoxResult.Text = prognostication.Predict(user);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            StoredResult sr = prognostication.NewModel(Convert.ToInt32(comboBoxNumberTest.Text));
            txtBoxResult.Clear();
            btnPredict.Enabled = true;
            labelAlg.Text = sr.algorithm;
            labelCountClusters.Text = sr.countClusters.ToString();
            labelStay.Text = sr.stayedPlayers[2].ToString();
            labelLeave.Text = sr.leavePlayers[2].ToString();
        }

        private void comboBoxNumberTest_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnPredict.Enabled = false;
        }

        private void dataGridFeatures_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            TextBox tb = (TextBox)e.Control;
            tb.KeyPress += new KeyPressEventHandler(tb_KeyPress);
        }
        void tb_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (!(Char.IsDigit(e.KeyChar)) && !((e.KeyChar == ',')))
            {
                if (e.KeyChar != (char)Keys.Back)
                {
                    e.Handled = true;
                }
            }
        }

        private void btnJson_Click(object sender, EventArgs e)
        {
            string path = "Save\\Json\\labels";
            int nmb = Convert.ToInt32(cmbBoxJsonTest.Text);
            string fileName = path + countTest + ".txt";
            DataStore.SerializeJSON(fileName, nmb);
        }


    }
}
