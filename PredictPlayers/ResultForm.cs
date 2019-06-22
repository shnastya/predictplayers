using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PredictPlayers
{
    public partial class ResultForm : Form
    {
        int numbTest;
        int countParams = 5;

        public ResultForm(int numbTest)
        {
            InitializeComponent();
            this.numbTest = numbTest;
        }

        private void ResultForm_Load(object sender, EventArgs e)
        {
            FillTables();
            listBoxMetrics.Items.Add("Presicion -> 1");
            listBoxMetrics.Items.Add("Recall -> 1");
            listBoxMetrics.Items.Add("f-score -> 1");
            listBoxMetrics.Items.Add("Silhouette -> 1");
            listBoxMetrics.Items.Add("Q -> 0");
            listBoxMetrics.Items.Add("β -> 1");
            listBoxMetrics.Items.Add("%ошибок -> 0");
        }

        private void FillTables()
        {
            StoredResult sr = DataStore.Deserialize(numbTest);
            int ind = gridMatrixErr.Rows.Add();
            gridMatrixErr.Rows[ind].Cells[0].Value = "Остался";
            gridMatrixErr.Rows[ind].Cells[1].Value = sr.stayedPlayers[0];
            gridMatrixErr.Rows[ind].Cells[2].Value = sr.stayedPlayers[1];
            gridMatrixErr.Rows[ind].Cells[3].Value = sr.stayedPlayers[2];
            ind = gridMatrixErr.Rows.Add();
            gridMatrixErr.Rows[ind].Cells[0].Value = "Ушел";
            gridMatrixErr.Rows[ind].Cells[1].Value = sr.leavePlayers[0];
            gridMatrixErr.Rows[ind].Cells[2].Value = sr.leavePlayers[1];
            gridMatrixErr.Rows[ind].Cells[3].Value = sr.leavePlayers[2];

            ind = gridMetrics.Rows.Add();
            gridMetrics.Rows[ind].Cells[0].Value = sr.silhouette;
            gridMetrics.Rows[ind].Cells[1].Value = sr.Qmetrics;
            gridMetrics.Rows[ind].Cells[2].Value = sr.Bmetrics;

            for(int i = 0; i < sr.countClusters; i++)
            {
                ind = gridPercent.Rows.Add();
                gridPercent.Rows[ind].Cells[0].Value = sr.clusters[i].number;
                gridPercent.Rows[ind].Cells[1].Value = sr.clusters[i].leaveCount;
                gridPercent.Rows[ind].Cells[2].Value = sr.clusters[i].stayCount;
                gridPercent.Rows[ind].Cells[3].Value = sr.clusters[i].metka;
                gridPercent.Rows[ind].Cells[4].Value = sr.clusters[i].errors;

            }

            ind = gridParams.Rows.Add();
            gridParams[0, ind].Value = "Алгоритм";
            gridParams[1, ind].Value = sr.algorithm;
            ind = gridParams.Rows.Add();
            gridParams[0, ind].Value = "Количество кластеров";
            gridParams[1, ind].Value = sr.countClusters;
            ind = gridParams.Rows.Add();
            gridParams[0, ind].Value = "Нормализация";
            if (sr.normalization)
                gridParams[1, ind].Value = "Да";
            else gridParams[1, ind].Value = "Нет";

            switch (sr.algorithm)
            {
                case "Hierarchical agglomerative":
                    ind = gridParams.Rows.Add();
                    gridParams[0, ind].Value = "Метрика";
                    gridParams[1, ind].Value = sr.metrcis;
                    ind = gridParams.Rows.Add();
                    gridParams[0, ind].Value = "Связь";
                    gridParams[1, ind].Value = sr.link;
                    break;
                case "KMeans":
                    ind = gridParams.Rows.Add();
                    gridParams[0, ind].Value = "Метод инициализации";
                    gridParams[1, ind].Value = sr.init;

                    ind = gridParams.Rows.Add();
                    gridParams[0, ind].Value = "Алгоритм";
                    gridParams[1, ind].Value = sr.alg;
                    break;
                case "DBSCAN":
                    ind = gridParams.Rows.Add();
                    gridParams[0, ind].Value = "Eps-окрестность";
                    gridParams[1, ind].Value = sr.eps;
                    ind = gridParams.Rows.Add();
                    gridParams[0, ind].Value = "Min samples";
                    gridParams[1, ind].Value = sr.minSample;
                    break;
                case "Affinity Propagation":
                    ind = gridParams.Rows.Add();
                    gridParams[0, ind].Value = "Damping коэффициент";
                    gridParams[1, ind].Value = sr.damping;
                    ind = gridParams.Rows.Add();
                    gridParams[0, ind].Value = "Max_iter";
                    gridParams[1, ind].Value = sr.maxIter;
                    ind = gridParams.Rows.Add();
                    gridParams[0, ind].Value = "Convergence_iter";
                    gridParams[1, ind].Value = sr.convergIter;
                    break;
            }
        }
        
    }
}
