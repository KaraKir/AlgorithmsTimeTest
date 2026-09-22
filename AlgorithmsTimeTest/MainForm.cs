using AlgorithmApp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace AlgorithmsTimeTest
{
    public class MainForm : Form
    {
        // ---------- Одиночный запуск ----------
        private TextBox txtFilePath;
        private Button btnBrowse;
        private ComboBox cmbAlgorithms;
        private Button btnRun;
        private Button btnSave;
        private TextBox txtLog;
        private Label lblStatus;

        // ---------- Серия по файлу ----------
        private NumericUpDown numFileNFrom, numFileNTo, numFileStep, numFileRuns;
        private Button btnRunFileSeries;

        private ResultRecord lastResult;

        // ---------- График 2D ----------
        private Chart chart;

        // ---------- Панель эксперимента ----------
        private ComboBox cmbAlgoRegistry;
        private NumericUpDown numNFrom, numNTo, numStep, numRuns;
        private Button btnRunSeries, btnLoadFromDb, btnClearDb;
        private CheckBox chkComparePow;
        private Button btn3DMatrices, btnAutoNMax, btnExportReport;
        private Button btnShowAll;
        private Button btnRunAll;
        private Button btnCancelRunAll;
        private Button btnClearAllDb;      // <-- новая кнопка
        private bool cancelRunAll;
        private ProgressBar progressBar;
        private Label lblProgress;

        public MainForm()
        {
            Text = "Анализ временной сложности алгоритмов";
            Width = 1180;
            Height = 900;
            StartPosition = FormStartPosition.CenterScreen;

            InitializeControls();

            try
            {
                Database.Init();
                Log("БД experiment.db готова.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось инициализировать БД: " + ex.ToString(),
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeControls()
        {
            // ======================== Файл ========================
            var lblFile = new Label
            {
                Text = "Файл входных данных:",
                Location = new Point(15, 15),
                AutoSize = true
            };

            txtFilePath = new TextBox
            {
                Location = new Point(15, 38),
                Width = 800,
                ReadOnly = true
            };

            btnBrowse = new Button
            {
                Text = "Обзор...",
                Location = new Point(825, 36),
                Width = 120
            };
            btnBrowse.Click += BtnBrowse_Click;

            // ================== Одиночный запуск ==================
            var lblAlgo = new Label
            {
                Text = "Алгоритм (одиночный запуск):",
                Location = new Point(15, 75),
                AutoSize = true
            };

            cmbAlgorithms = new ComboBox
            {
                Location = new Point(15, 98),
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbAlgorithms.Items.Add("Сортировка массива");
            cmbAlgorithms.Items.Add("Поиск простых чисел");
            cmbAlgorithms.Items.Add("Сумма элементов");
            cmbAlgorithms.Items.Add("Произведение элементов");
            cmbAlgorithms.Items.Add("Полином (наивно)");
            cmbAlgorithms.Items.Add("Полином (Горнер)");
            cmbAlgorithms.Items.Add("Сортировка пузырьком");
            cmbAlgorithms.Items.Add("Быстрая сортировка");
            cmbAlgorithms.Items.Add("Timsort (стандартная сортировка)");
            cmbAlgorithms.Items.Add("Timsort (собственная реализация)");
            cmbAlgorithms.Items.Add("Сортировка слиянием");
            cmbAlgorithms.Items.Add("Факториал первого элемента");
            cmbAlgorithms.SelectedIndex = 0;

            btnRun = new Button
            {
                Text = "Выполнить",
                Location = new Point(330, 96),
                Width = 100,
                Enabled = false
            };
            btnRun.Click += BtnRun_Click;

            btnSave = new Button
            {
                Text = "Сохранить результат",
                Location = new Point(435, 96),
                Width = 130,
                Enabled = false
            };
            btnSave.Click += BtnSave_Click;

            // ================== Серия по файлу ==================
            var lblFileSeries = new Label
            {
                Text = "Серия по файлу (n от / до / шаг / запусков):",
                Location = new Point(15, 135),
                AutoSize = true
            };

            numFileNFrom = new NumericUpDown
            {
                Location = new Point(280, 132),
                Width = 70,
                Minimum = 1,
                Maximum = 1000000,
                Value = 10
            };
            numFileNTo = new NumericUpDown
            {
                Location = new Point(355, 132),
                Width = 80,
                Minimum = 1,
                Maximum = 1000000,
                Value = 1000
            };
            numFileStep = new NumericUpDown
            {
                Location = new Point(440, 132),
                Width = 70,
                Minimum = 1,
                Maximum = 100000,
                Value = 50
            };
            numFileRuns = new NumericUpDown
            {
                Location = new Point(515, 132),
                Width = 60,
                Minimum = 1,
                Maximum = 50,
                Value = 5
            };

            btnRunFileSeries = new Button
            {
                Text = "Серия по файлу",
                Location = new Point(585, 130),
                Width = 160,
                Height = 26,
                Enabled = false
            };
            btnRunFileSeries.Click += BtnRunFileSeries_Click;

            cmbAlgorithms.SelectedIndexChanged += (s, e) =>
            {
                UpdateRunFileSeriesButton();
            };

            // ======================== Журнал ========================
            var lblLog = new Label
            {
                Text = "Журнал:",
                Location = new Point(15, 170),
                AutoSize = true
            };

            txtLog = new TextBox
            {
                Location = new Point(15, 193),
                Width = 1120,
                Height = 90,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                Font = new Font("Consolas", 9)
            };

            // ======================== График ========================
            var lblChart = new Label
            {
                Text = "График:",
                Location = new Point(15, 288),
                AutoSize = true
            };

            chart = new Chart
            {
                Location = new Point(15, 310),
                Width = 1120,
                Height = 250,
                BackColor = Color.WhiteSmoke,
                BorderlineColor = Color.LightGray,
                BorderlineDashStyle = ChartDashStyle.Solid,
                BorderlineWidth = 1
            };

            var area = new ChartArea("main");
            area.AxisX.MajorGrid.LineColor = Color.LightGray;
            area.AxisY.MajorGrid.LineColor = Color.LightGray;
            area.AxisX.Title = "Размер входа n";
            area.AxisY.Title = "Значение";
            area.BackColor = Color.White;
            chart.ChartAreas.Add(area);

            chart.Titles.Add(new Title(
                "Эксперимент и аппроксимация T(n) = C · f(n)",
                Docking.Top,
                new Font("Segoe UI", 10, FontStyle.Bold),
                Color.DimGray));

            // =================== Панель эксперимента ===================
            var lblAlgoReg = new Label
            {
                Text = "Алгоритм (серия замеров):",
                Location = new Point(15, 575),
                AutoSize = true
            };

            cmbAlgoRegistry = new ComboBox
            {
                Location = new Point(15, 598),
                Width = 250,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            foreach (var a in AlgorithmRegistry.All)
                cmbAlgoRegistry.Items.Add(a.Display);
            cmbAlgoRegistry.SelectedIndex = 0;

            cmbAlgoRegistry.SelectedIndexChanged += (s, e) =>
            {
                if (cmbAlgoRegistry.SelectedIndex < 0) return;
                var algo = AlgorithmRegistry.All[cmbAlgoRegistry.SelectedIndex];
                var range = GetDefaultRange(algo);
                numNFrom.Value = Math.Min(range.nFrom, numNFrom.Maximum);
                numNTo.Value = Math.Min(range.nTo, numNTo.Maximum);
                numStep.Value = Math.Min(range.step, numStep.Maximum);
                numRuns.Value = Math.Min(range.runs, numRuns.Maximum);
            };

            var lblNFrom = new Label { Text = "n от:", Location = new Point(280, 575), AutoSize = true };
            var lblNTo = new Label { Text = "до:", Location = new Point(370, 575), AutoSize = true };
            var lblStep = new Label { Text = "шаг:", Location = new Point(460, 575), AutoSize = true };
            var lblRuns = new Label { Text = "запусков:", Location = new Point(550, 575), AutoSize = true };

            numNFrom = new NumericUpDown
            {
                Location = new Point(280, 598),
                Width = 80,
                Minimum = 1,
                Maximum = 100000,
                Value = 10
            };
            numNTo = new NumericUpDown
            {
                Location = new Point(370, 598),
                Width = 80,
                Minimum = 1,
                Maximum = 100000,
                Value = 100
            };
            numStep = new NumericUpDown
            {
                Location = new Point(460, 598),
                Width = 80,
                Minimum = 1,
                Maximum = 10000,
                Value = 10
            };
            numRuns = new NumericUpDown
            {
                Location = new Point(550, 598),
                Width = 80,
                Minimum = 1,
                Maximum = 50,
                Value = 5
            };

            btnRunSeries = new Button
            {
                Text = "Запустить серию",
                Location = new Point(650, 596),
                Width = 160,
                Height = 28
            };
            btnRunSeries.Click += BtnRunSeries_Click;

            btnLoadFromDb = new Button
            {
                Text = "Загрузить из БД",
                Location = new Point(820, 596),
                Width = 150,
                Height = 28
            };
            btnLoadFromDb.Click += BtnLoadFromDb_Click;

            btnClearDb = new Button
            {
                Text = "Очистить БД алгоритма",
                Location = new Point(980, 596),
                Width = 155,
                Height = 28
            };
            btnClearDb.Click += BtnClearDb_Click;

            // =================== Чекбокс сравнения Pow ===================
            chkComparePow = new CheckBox
            {
                Text = "Сравнить все три Pow на одном графике (логарифмическая шкала)",
                Location = new Point(15, 632),
                Width = 650,
                AutoSize = true
            };
            chkComparePow.CheckedChanged += (s, e) =>
            {
                cmbAlgoRegistry.Enabled = !chkComparePow.Checked;
            };

            // =================== Кнопка 3D-эксперимента ===================
            btn3DMatrices = new Button
            {
                Text = "3D-эксперимент: матрицы (heatmap)",
                Location = new Point(430, 630),
                Width = 250,
                Height = 28
            };
            btn3DMatrices.Click += Btn3DMatrices_Click;

            // =================== Кнопка автоподбора N_max ===================
            btnAutoNMax = new Button
            {
                Text = "Подобрать N_max",
                Location = new Point(690, 630),
                Width = 130,
                Height = 28
            };
            btnAutoNMax.Click += BtnAutoNMax_Click;

            // =================== Кнопка экспорта отчёта ===================
            btnExportReport = new Button
            {
                Text = "Экспорт отчёта",
                Location = new Point(825, 630),
                Width = 90,
                Height = 28
            };
            btnExportReport.Click += BtnExportReport_Click;

            // =================== Кнопка «Очистить всю БД» ===================
            btnClearAllDb = new Button
            {
                Text = "Очистить всю БД",
                Location = new Point(925, 630),
                Width = 130,
                Height = 28,
                ForeColor = Color.DarkRed
            };
            btnClearAllDb.Click += BtnClearAllDb_Click;

            // =================== Кнопка «Все замеры» ===================
            btnShowAll = new Button
            {
                Text = "Все замеры",
                Location = new Point(15, 660),
                Width = 120,
                Height = 28
            };
            btnShowAll.Click += BtnShowAll_Click;

            // =================== Запуск всех и сохранение ===================
            btnRunAll = new Button
            {
                Text = "Запустить все и сохранить",
                Location = new Point(145, 660),
                Width = 220,
                Height = 28
            };
            btnRunAll.Click += BtnRunAll_Click;

            btnCancelRunAll = new Button
            {
                Text = "Отмена",
                Location = new Point(370, 660),
                Width = 80,
                Height = 28,
                Enabled = false
            };
            btnCancelRunAll.Click += (s, e) => { cancelRunAll = true; };

            // =================== Прогресс ===================
            progressBar = new ProgressBar
            {
                Location = new Point(15, 700),
                Width = 1120,
                Height = 20,
                Minimum = 0,
                Maximum = 100
            };

            lblProgress = new Label
            {
                Text = "Готово",
                Location = new Point(15, 726),
                AutoSize = true,
                ForeColor = Color.DimGray
            };

            lblStatus = new Label
            {
                Text = "Готово",
                Location = new Point(15, 750),
                AutoSize = true,
                ForeColor = Color.DarkGreen
            };

            Controls.AddRange(new Control[]
            {
                lblFile, txtFilePath, btnBrowse,
                lblAlgo, cmbAlgorithms, btnRun, btnSave,
                lblFileSeries, numFileNFrom, numFileNTo, numFileStep, numFileRuns, btnRunFileSeries,
                lblLog, txtLog,
                lblChart, chart,
                lblAlgoReg, cmbAlgoRegistry,
                lblNFrom, numNFrom, lblNTo, numNTo,
                lblStep, numStep, lblRuns, numRuns,
                btnRunSeries, btnLoadFromDb, btnClearDb,
                chkComparePow, btn3DMatrices, btnAutoNMax, btnExportReport, btnClearAllDb, btnShowAll,
                btnRunAll, btnCancelRunAll,
                progressBar, lblProgress, lblStatus
            });

            UpdateRunFileSeriesButton();
        }

        private void UpdateRunFileSeriesButton()
        {
            if (btnRunFileSeries == null) return;
            if (cmbAlgorithms == null || cmbAlgorithms.SelectedItem == null)
            {
                btnRunFileSeries.Enabled = false;
                return;
            }

            string display = cmbAlgorithms.SelectedItem.ToString();
            var info = AlgorithmRegistry.All.Find(a => a.Display == display);
            bool compatible = info != null && info.FileSeriesCompatible;
            bool fileChosen = !string.IsNullOrWhiteSpace(txtFilePath.Text) &&
                              File.Exists(txtFilePath.Text);

            btnRunFileSeries.Enabled = compatible && fileChosen;
        }

        // ============================================================
        // ================   ОДИНОЧНЫЙ ЗАПУСК   ======================
        // ============================================================

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
                ofd.Title = "Выберите файл с входными данными";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtFilePath.Text = ofd.FileName;
                    btnRun.Enabled = true;
                    UpdateRunFileSeriesButton();
                    Log($"Выбран файл: {ofd.FileName}");
                }
            }
        }

        private int[] ReadInputFile(string path)
        {
            var text = File.ReadAllText(path);
            var parts = text.Split(new[] { ' ', '\t', '\r', '\n', ',', ';' },
                                   StringSplitOptions.RemoveEmptyEntries);

            var numbers = new List<int>();
            foreach (var p in parts)
            {
                if (int.TryParse(p, out int value))
                    numbers.Add(value);
            }

            if (numbers.Count == 0)
                throw new InvalidDataException("Файл не содержит корректных чисел.");

            return numbers.ToArray();
        }

        private void BtnRun_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFilePath.Text) ||
                !File.Exists(txtFilePath.Text))
            {
                MessageBox.Show("Сначала выберите корректный файл.",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                btnRun.Enabled = false;
                lblStatus.Text = "Выполняется...";
                lblStatus.ForeColor = Color.DarkOrange;

                int[] data = ReadInputFile(txtFilePath.Text);
                string algorithm = cmbAlgorithms.SelectedItem.ToString();

                string summary;
                double ms;

                switch (algorithm)
                {
                    case "Сортировка массива":
                        {
                            var r = Algorithms.SortArray((int[])data.Clone());
                            summary = r.result; ms = r.ms; break;
                        }
                    case "Поиск простых чисел":
                        {
                            var r = Algorithms.FindPrimes(data);
                            summary = r.result; ms = r.ms; break;
                        }
                    case "Сумма элементов":
                        {
                            var r = Algorithms.SumArray(data);
                            summary = r.result; ms = r.ms; break;
                        }
                    case "Факториал первого элемента":
                        {
                            var r = Algorithms.Factorial(data);
                            summary = r.result; ms = r.ms; break;
                        }
                    case "Произведение элементов":
                        {
                            var sw = System.Diagnostics.Stopwatch.StartNew();
                            long p = AlgorithmRegistry.ProductVector(data);
                            sw.Stop();
                            summary = $"Произведение = {p}";
                            ms = sw.Elapsed.TotalMilliseconds;
                            break;
                        }
                    case "Полином (наивно)":
                        {
                            var sw = System.Diagnostics.Stopwatch.StartNew();
                            double v = AlgorithmRegistry.PolyNaive(data, 1.5);
                            sw.Stop();
                            summary = $"P(1.5) = {v:G6} (наивно)";
                            ms = sw.Elapsed.TotalMilliseconds;
                            break;
                        }
                    case "Полином (Горнер)":
                        {
                            var sw = System.Diagnostics.Stopwatch.StartNew();
                            double v = AlgorithmRegistry.PolyHorner(data, 1.5);
                            sw.Stop();
                            summary = $"P(1.5) = {v:G6} (Горнер)";
                            ms = sw.Elapsed.TotalMilliseconds;
                            break;
                        }
                    case "Сортировка пузырьком":
                        {
                            var copy = (int[])data.Clone();
                            var sw = System.Diagnostics.Stopwatch.StartNew();
                            AlgorithmRegistry.BubbleSort(copy);
                            sw.Stop();
                            summary = $"Отсортировано {copy.Length} элементов (пузырёк). Первые: " +
                                      string.Join(", ", copy.Take(5));
                            ms = sw.Elapsed.TotalMilliseconds;
                            break;
                        }
                    case "Быстрая сортировка":
                        {
                            var copy = (int[])data.Clone();
                            var sw = System.Diagnostics.Stopwatch.StartNew();
                            AlgorithmRegistry.QuickSort(copy, 0, copy.Length - 1);
                            sw.Stop();
                            summary = $"Отсортировано {copy.Length} элементов (QuickSort). Первые: " +
                                      string.Join(", ", copy.Take(5));
                            ms = sw.Elapsed.TotalMilliseconds;
                            break;
                        }
                    case "Timsort (стандартная сортировка)":
                        {
                            var copy = (int[])data.Clone();
                            var sw = System.Diagnostics.Stopwatch.StartNew();
                            Array.Sort(copy);
                            sw.Stop();
                            summary = $"Отсортировано {copy.Length} элементов (Array.Sort). Первые: " +
                                      string.Join(", ", copy.Take(5));
                            ms = sw.Elapsed.TotalMilliseconds;
                            break;
                        }
                    case "Timsort (собственная реализация)":
                        {
                            var copy = (int[])data.Clone();
                            var sw = System.Diagnostics.Stopwatch.StartNew();
                            AlgorithmRegistry.Timsort(copy);
                            sw.Stop();
                            summary = $"Отсортировано {copy.Length} элементов (Timsort). Первые: " +
                                      string.Join(", ", copy.Take(5));
                            ms = sw.Elapsed.TotalMilliseconds;
                            break;
                        }
                    case "Сортировка слиянием":
                        {
                            var copy = (int[])data.Clone();
                            var sw = System.Diagnostics.Stopwatch.StartNew();
                            AlgorithmRegistry.MergeSort(copy, 0, copy.Length - 1);
                            sw.Stop();
                            summary = $"Отсортировано {copy.Length} элементов (MergeSort). Первые: " +
                                      string.Join(", ", copy.Take(5));
                            ms = sw.Elapsed.TotalMilliseconds;
                            break;
                        }
                    default:
                        summary = "Неизвестный алгоритм";
                        ms = 0;
                        break;
                }

                lastResult = new ResultRecord
                {
                    AlgorithmName = algorithm,
                    ElapsedMilliseconds = ms,
                    InputFile = txtFilePath.Text,
                    Timestamp = DateTime.Now,
                    ResultSummary = summary
                };

                Log("---");
                Log($"Алгоритм: {algorithm}");
                Log($"Результат: {summary}");
                Log($"Затрачено времени: {ms:F2} мс");

                lblStatus.Text = $"Готово за {ms:F2} мс";
                lblStatus.ForeColor = Color.DarkGreen;
                btnSave.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка выполнения: " + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log("ОШИБКА: " + ex.Message);
                lblStatus.Text = "Ошибка";
                lblStatus.ForeColor = Color.DarkRed;
            }
            finally
            {
                Cursor = Cursors.Default;
                btnRun.Enabled = true;
            }
        }

        // ============================================================
        // =============   СЕРИЯ ПО ФАЙЛУ (ВАРИАНТ C)   ===============
        // ============================================================

        private void BtnRunFileSeries_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFilePath.Text) ||
                !File.Exists(txtFilePath.Text))
            {
                MessageBox.Show("Сначала выберите файл.",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string algorithm = cmbAlgorithms.SelectedItem.ToString();
            var algoInfo = AlgorithmRegistry.All.Find(a => a.Display == algorithm);

            if (algoInfo == null || !algoInfo.FileSeriesCompatible)
            {
                MessageBox.Show(
                    "Этот алгоритм нельзя прогнать в серии по файлу.\n\n" +
                    "Серия по файлу работает только с векторными алгоритмами:\n" +
                    "  • Сумма элементов\n" +
                    "  • Произведение элементов\n" +
                    "  • Сортировка пузырьком\n" +
                    "  • Быстрая сортировка\n" +
                    "  • Timsort (стандартная и собственная)\n" +
                    "  • Сортировка слиянием",
                    "Недоступно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int nFrom = (int)numFileNFrom.Value;
            int nTo = (int)numFileNTo.Value;
            int step = (int)numFileStep.Value;
            int runs = (int)numFileRuns.Value;

            if (nTo < nFrom || step <= 0)
            {
                MessageBox.Show("Проверьте диапазон n и шаг.");
                return;
            }

            int[] sourceData;
            try
            {
                sourceData = ReadInputFile(txtFilePath.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка чтения файла: " + ex.Message);
                return;
            }

            int fileSize = sourceData.Length;
            if (nTo > fileSize) nTo = fileSize;
            if (nFrom > fileSize)
            {
                MessageBox.Show(
                    $"Файл содержит только {fileSize} чисел. Уменьшите «n от».",
                    "Недостаточно данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnRunFileSeries.Enabled = false;
            btnRun.Enabled = false;
            progressBar.Value = 0;
            lblStatus.Text = "Серия по файлу...";
            lblStatus.ForeColor = Color.DarkOrange;

            string dbKey = "File:" + algoInfo.Key;
            string capturedAlgorithm = algorithm;

            Task.Run(() =>
            {
                try
                {
                    var byN = new Dictionary<int, List<double>>();
                    int total = (nTo - nFrom) / step + 1;
                    int done = 0;

                    for (int n = nFrom; n <= nTo; n += step)
                    {
                        var slice = new int[n];
                        Array.Copy(sourceData, slice, n);

                        try { algoInfo.Run(CloneInput(slice)); } catch { }

                        for (int r = 1; r <= runs; r++)
                        {
                            var sw = System.Diagnostics.Stopwatch.StartNew();
                            algoInfo.Run(CloneInput(slice));
                            sw.Stop();

                            double ms = sw.Elapsed.TotalMilliseconds;

                            var m = new Measurement
                            {
                                Algorithm = dbKey,
                                N = n,
                                Value = ms,
                                RunIndex = r,
                                Timestamp = DateTime.Now
                            };
                            Database.SaveMeasurement(m);

                            if (!byN.ContainsKey(n)) byN[n] = new List<double>();
                            byN[n].Add(ms);
                        }

                        done++;
                        int percent = (int)(100.0 * done / total);
                        int nCaptured = n;

                        BeginInvoke(new Action(() =>
                        {
                            progressBar.Value = Math.Min(percent, 100);
                            lblProgress.Text = $"Файл: n = {nCaptured}, {done}/{total}";
                        }));
                    }

                    var data = MedianByN(byN);

                    var fit = Approximator.Fit(data, algoInfo.TheoreticalModel);

                    Database.SaveApproximation(new Approximation
                    {
                        Algorithm = dbKey,
                        Model = algoInfo.TheoreticalModel,
                        C = fit.C,
                        MSE = fit.MSE,
                        Timestamp = DateTime.Now
                    });

                    BeginInvoke(new Action(() =>
                    {
                        PlotResults(dbKey, data, algoInfo.TheoreticalModel, fit.C);

                        Log("---");
                        Log($"Серия по файлу: {capturedAlgorithm}");
                        Log($"Точек: {data.Count}, C = {fit.C:G4}, MSE = {fit.MSE:G4}");

                        lblStatus.Text = $"Готово. C = {fit.C:G4}, MSE = {fit.MSE:G4}";
                        lblStatus.ForeColor = Color.DarkGreen;
                        progressBar.Value = 100;
                        UpdateRunFileSeriesButton();
                        btnRun.Enabled = true;
                    }));
                }
                catch (Exception ex)
                {
                    BeginInvoke(new Action(() =>
                    {
                        MessageBox.Show("Ошибка: " + ex.Message,
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        UpdateRunFileSeriesButton();
                        btnRun.Enabled = true;
                    }));
                }
            });
        }

        // ============================================================
        // ==============   СОХРАНЕНИЕ РЕЗУЛЬТАТА   ===================
        // ============================================================

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (lastResult == null) return;

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "Текстовые файлы (*.txt)|*.txt|CSV файлы (*.csv)|*.csv";
                sfd.FileName = $"result_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var lines = new[]
                        {
                            "=== Результат вычислений ===",
                            $"Дата и время:      {lastResult.Timestamp:yyyy-MM-dd HH:mm:ss}",
                            $"Тип вычислений:    {lastResult.AlgorithmName}",
                            $"Файл данных:       {lastResult.InputFile}",
                            $"Результат:         {lastResult.ResultSummary}",
                            $"Затрачено времени: {lastResult.ElapsedMilliseconds:F2} мс"
                        };

                        File.WriteAllLines(sfd.FileName, lines);
                        Log($"Результат сохранён: {sfd.FileName}");

                        MessageBox.Show("Результат успешно сохранён.",
                            "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка сохранения: " + ex.Message,
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // ============================================================
        // ==============   СЕРИЯ ЗАМЕРОВ (генерация)   ===============
        // ============================================================

        private void BtnRunSeries_Click(object sender, EventArgs e)
        {
            if (chkComparePow != null && chkComparePow.Checked)
            {
                RunPowComparison();
                return;
            }

            if (cmbAlgoRegistry.SelectedIndex < 0) return;

            var algo = AlgorithmRegistry.All[cmbAlgoRegistry.SelectedIndex];
            int nFrom = (int)numNFrom.Value;
            int nTo = (int)numNTo.Value;
            int step = (int)numStep.Value;
            int runs = (int)numRuns.Value;

            if (nTo < nFrom)
            {
                MessageBox.Show("«n до» должно быть >= «n от».");
                return;
            }
            if (step <= 0)
            {
                MessageBox.Show("Шаг должен быть положительным.");
                return;
            }

            if (Database.HasMeasurements(algo.Key))
            {
                var ans = MessageBox.Show(
                    "Для этого алгоритма уже есть замеры в БД.\n\n" +
                    "Да — удалить старые и замерить заново.\n" +
                    "Нет — показать существующие (кэш).\n" +
                    "Отмена — ничего не делать.",
                    "Кэш замеров",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (ans == DialogResult.Cancel) return;
                if (ans == DialogResult.No)
                {
                    LoadAndPlot(algo.Key);
                    return;
                }
                Database.ClearAlgorithm(algo.Key);
            }

            btnRunSeries.Enabled = false;
            progressBar.Value = 0;
            lblProgress.Text = "Запуск...";

            Task.Run(() =>
            {
                try
                {
                    RunSeriesInternal(algo, nFrom, nTo, step, runs);
                }
                catch (Exception ex)
                {
                    BeginInvoke(new Action(() =>
                    {
                        MessageBox.Show("Ошибка: " + ex.Message,
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        lblProgress.Text = "Ошибка";
                    }));
                }
                finally
                {
                    BeginInvoke(new Action(() =>
                    {
                        btnRunSeries.Enabled = true;
                    }));
                }
            });
        }

        private void RunSeriesInternal(
            AlgorithmRegistry.AlgorithmInfo algo,
            int nFrom, int nTo, int step, int runs)
        {
            int total = (nTo - nFrom) / step + 1;
            int done = 0;
            var byN = new Dictionary<int, List<double>>();

            for (int n = nFrom; n <= nTo; n += step)
            {
                var input = algo.GenerateInput(n);
                try { algo.Run(CloneInput(input)); } catch { }

                for (int run = 1; run <= runs; run++)
                {
                    double value;
                    if (algo.Key.StartsWith("Pow"))
                        value = MeasureStepsFor(algo.Key, n);
                    else
                    {
                        var sw = System.Diagnostics.Stopwatch.StartNew();
                        algo.Run(CloneInput(input));
                        sw.Stop();
                        value = sw.Elapsed.TotalMilliseconds;
                    }

                    var m = new Measurement
                    {
                        Algorithm = algo.Key,
                        N = n,
                        Value = value,
                        RunIndex = run,
                        Timestamp = DateTime.Now
                    };
                    Database.SaveMeasurement(m);

                    if (!byN.ContainsKey(n)) byN[n] = new List<double>();
                    byN[n].Add(value);
                }

                done++;
                int percent = (int)(100.0 * done / total);
                int nCaptured = n;

                BeginInvoke(new Action(() =>
                {
                    progressBar.Value = Math.Min(percent, 100);
                    lblProgress.Text = $"n = {nCaptured}, прогресс {done}/{total}";
                }));
            }

            var data = MedianByN(byN);

            var fit = Approximator.Fit(data, algo.TheoreticalModel);

            Database.SaveApproximation(new Approximation
            {
                Algorithm = algo.Key,
                Model = algo.TheoreticalModel,
                C = fit.C,
                MSE = fit.MSE,
                Timestamp = DateTime.Now
            });

            BeginInvoke(new Action(() =>
            {
                PlotResults(algo.Key, data, algo.TheoreticalModel, fit.C);
                Log($"Серия завершена: {algo.Display}");
                Log($"C = {fit.C:G4}, MSE = {fit.MSE:G4}");
                lblProgress.Text = $"Готово. C = {fit.C:G4}, MSE = {fit.MSE:G4}";
            }));
        }

        private List<(int N, double T)> MedianByN(Dictionary<int, List<double>> byN)
        {
            var data = new List<(int N, double T)>();
            foreach (var kv in byN)
            {
                var sorted = new List<double>(kv.Value);
                sorted.Sort();
                double median = sorted.Count % 2 == 1
                    ? sorted[sorted.Count / 2]
                    : 0.5 * (sorted[sorted.Count / 2 - 1] + sorted[sorted.Count / 2]);
                data.Add((kv.Key, median));
            }
            data.Sort((a, b) => a.N.CompareTo(b.N));
            return data;
        }

        private object CloneInput(object input)
        {
            if (input is int[] v) return (int[])v.Clone();
            if (input is int[,] m) return (int[,])m.Clone();
            return input;
        }

        private double MeasureStepsFor(string key, int n)
        {
            switch (key)
            {
                case "PowNaive": return AlgorithmRegistry.PowNaiveSteps(1.0001, n);
                case "PowRecursive": return AlgorithmRegistry.PowRecursiveSteps(1.0001, n);
                case "PowFast": return AlgorithmRegistry.PowFastSteps(1.0001, n);
                default: return 0;
            }
        }

        private void BtnLoadFromDb_Click(object sender, EventArgs e)
        {
            if (cmbAlgoRegistry.SelectedIndex < 0) return;
            var algo = AlgorithmRegistry.All[cmbAlgoRegistry.SelectedIndex];

            if (!Database.HasMeasurements(algo.Key))
            {
                MessageBox.Show("В БД нет замеров для этого алгоритма.",
                    "Пусто", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            LoadAndPlot(algo.Key);
        }

        private void BtnClearDb_Click(object sender, EventArgs e)
        {
            if (cmbAlgoRegistry.SelectedIndex < 0) return;
            var algo = AlgorithmRegistry.All[cmbAlgoRegistry.SelectedIndex];

            var ans = MessageBox.Show(
                $"Удалить все замеры для «{algo.Display}» из БД?",
                "Подтверждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (ans == DialogResult.Yes)
            {
                Database.ClearAlgorithm(algo.Key);
                Log("БД очищена для " + algo.Key);
            }
        }

        // ============ Новая кнопка: очистить всю БД ============
        private void BtnClearAllDb_Click(object sender, EventArgs e)
        {
            var ans = MessageBox.Show(
                "Удалить ВСЕ замеры и аппроксимации из БД?\n\n" +
                "Это действие нельзя отменить. Все ранее сохранённые " +
                "результаты серий замеров будут потеряны.\n\n" +
                "Продолжить?",
                "Полная очистка БД",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (ans != DialogResult.Yes) return;

            try
            {
                Database.ClearAll();
                Log("БД полностью очищена.");

                // Очистим график и заголовок
                chart.Series.Clear();
                chart.Titles.Clear();
                chart.Titles.Add(new Title(
                    "График (БД пуста)",
                    Docking.Top, new Font("Segoe UI", 10, FontStyle.Bold), Color.DimGray));

                lblProgress.Text = "БД очищена.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка очистки БД: " + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAndPlot(string algoKey)
        {
            var data = Database.GetAverages(algoKey);
            if (data.Count == 0)
            {
                Log("Нет данных для " + algoKey);
                return;
            }

            var algo = AlgorithmRegistry.All.Find(a => a.Key == algoKey);
            if (algo == null)
            {
                Log("Алгоритм не найден в реестре: " + algoKey);
                return;
            }

            var fit = Approximator.Fit(data, algo.TheoreticalModel);

            PlotResults(algoKey, data, algo.TheoreticalModel, fit.C);
            Log($"Из БД: {data.Count} точек, C = {fit.C:G4}, MSE = {fit.MSE:G4}");
            lblProgress.Text = $"Кэш: C = {fit.C:G4}, MSE = {fit.MSE:G4}";
        }

        private void PlotResults(string algoKey, List<(int N, double T)> data,
                                 string model, double C)
        {
            chart.Series.Clear();

            var area = chart.ChartAreas[0];
            area.Area3DStyle.Enable3D = false;

            bool isSteps = algoKey.StartsWith("Pow");

            var expSeries = new Series("Экспериментальные результаты")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.SteelBlue,
                BorderWidth = 2,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 5,
                MarkerColor = Color.SteelBlue
            };
            foreach (var p in data)
            {
                double y = isSteps && p.T <= 0 ? 0.5 : p.T;
                expSeries.Points.AddXY(p.N, y);
            }
            chart.Series.Add(expSeries);

            var f = Approximator.ModelFunction(model);
            var approxSeries = new Series($"Аппроксимация: C · {model}")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.OrangeRed,
                BorderWidth = 2,
                BorderDashStyle = ChartDashStyle.Dash
            };
            foreach (var p in data)
            {
                double y = C * f(p.N);
                if (isSteps && y <= 0) y = 0.5;
                approxSeries.Points.AddXY(p.N, y);
            }
            chart.Series.Add(approxSeries);

            area.AxisX.Title = "Размер входа n";
            area.AxisY.Title = isSteps ? "Количество операций" : "Время (мс)";
            area.AxisX.MajorGrid.LineColor = Color.LightGray;
            area.AxisY.MajorGrid.LineColor = Color.LightGray;

            int minN = data[0].N;
            int maxN = data[data.Count - 1].N;
            bool logX = minN > 0 && maxN / (double)minN > 100.0;
            area.AxisX.IsLogarithmic = logX;
            area.AxisY.IsLogarithmic = isSteps;

            double mse = Approximator.Fit(data, model).MSE;
            chart.Titles.Clear();
            chart.Titles.Add(new Title(
                $"{algoKey}:  T(n) ≈ {C:G4} · {model},   MSE = {mse:G4}",
                Docking.Top,
                new Font("Segoe UI", 10, FontStyle.Bold),
                Color.DimGray));
        }

        // ============================================================
        // ============   ЧАСТЬ IV: СРАВНЕНИЕ POW   ===================
        // ============================================================

        private void RunPowComparison()
        {
            btnRunSeries.Enabled = false;
            progressBar.Value = 0;
            lblProgress.Text = "Сравнение Pow...";

            Task.Run(() =>
            {
                try
                {
                    var keys = new[] { "PowNaive", "PowRecursive", "PowFast" };
                    var seriesData = new Dictionary<string, List<(int N, double T)>>();

                    int nFrom = 1, nTo = 1000, step = 1;

                    foreach (var key in keys)
                    {
                        var data = new List<(int N, double T)>();
                        for (int n = nFrom; n <= nTo; n += step)
                        {
                            double v = MeasureStepsFor(key, n);
                            data.Add((n, v));

                            int capturedN = n;
                            string capturedKey = key;
                            BeginInvoke(new Action(() =>
                            {
                                int percent = (int)(100.0 * (capturedN - nFrom) / (nTo - nFrom));
                                progressBar.Value = Math.Min(percent, 100);
                                lblProgress.Text = $"{capturedKey}: n = {capturedN}";
                            }));
                        }
                        seriesData[key] = data;
                    }

                    BeginInvoke(new Action(() =>
                    {
                        PlotPowComparison(seriesData);
                        Log("Сравнение Pow завершено.");
                        lblProgress.Text = "Готово.";
                        progressBar.Value = 100;
                        btnRunSeries.Enabled = true;
                    }));
                }
                catch (Exception ex)
                {
                    BeginInvoke(new Action(() =>
                    {
                        MessageBox.Show("Ошибка: " + ex.Message,
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        btnRunSeries.Enabled = true;
                    }));
                }
            });
        }

        private void PlotPowComparison(Dictionary<string, List<(int N, double T)>> series)
        {
            chart.Series.Clear();
            chart.ChartAreas[0].Area3DStyle.Enable3D = false;

            var area = chart.ChartAreas[0];
            area.AxisX.Title = "n";
            area.AxisY.Title = "Количество операций";
            area.AxisX.IsLogarithmic = false;
            area.AxisY.IsLogarithmic = true;
            area.AxisX.MajorGrid.LineColor = Color.LightGray;
            area.AxisY.MajorGrid.LineColor = Color.LightGray;

            var colors = new Dictionary<string, Color>
            {
                { "PowNaive",     Color.IndianRed },
                { "PowRecursive", Color.SeaGreen  },
                { "PowFast",      Color.SteelBlue }
            };

            var titles = new Dictionary<string, string>
            {
                { "PowNaive",     "Наивный O(n)"     },
                { "PowRecursive", "Рекурсивный O(n)" },
                { "PowFast",      "Быстрый O(log n)" }
            };

            foreach (var kv in series)
            {
                var s = new Series(titles[kv.Key])
                {
                    ChartType = SeriesChartType.Line,
                    Color = colors[kv.Key],
                    BorderWidth = 2
                };

                foreach (var p in kv.Value)
                {
                    double y = p.T <= 0 ? 0.5 : p.T;
                    s.Points.AddXY(p.N, y);
                }
                chart.Series.Add(s);
            }

            chart.Titles.Clear();
            chart.Titles.Add(new Title(
                "Сравнение алгоритмов возведения в степень (log-шкала по Y)",
                Docking.Top, new Font("Segoe UI", 10, FontStyle.Bold), Color.DimGray));
        }

        // ============================================================
        // ==========   ЧАСТЬ II: 3D HEATMAP МАТРИЦ   =================
        // ============================================================

        private void Btn3DMatrices_Click(object sender, EventArgs e)
        {
            const int nMax = 50;
            const int step = 5;
            const int inner = 20;
            const int runs = 3;

            btnRunSeries.Enabled = false;
            btn3DMatrices.Enabled = false;
            progressBar.Value = 0;
            lblProgress.Text = "3D-эксперимент...";

            Task.Run(() =>
            {
                try
                {
                    int cells = nMax / step;
                    double[,] heat = new double[cells, cells];
                    int[] ns = new int[cells];
                    int[] ms = new int[cells];
                    for (int i = 0; i < cells; i++) ns[i] = (i + 1) * step;
                    for (int j = 0; j < cells; j++) ms[j] = (j + 1) * step;

                    double totalCost = 0;
                    for (int i = 0; i < cells; i++)
                        for (int j = 0; j < cells; j++)
                            totalCost += (double)ns[i] * ms[j] * inner;

                    double doneCost = 0;

                    for (int i = 0; i < cells; i++)
                    {
                        int rowsA = ns[i];
                        for (int j = 0; j < cells; j++)
                        {
                            int colsB = ms[j];

                            var A = AlgorithmRegistry.GenerateMatrix(rowsA, inner);
                            var B = AlgorithmRegistry.GenerateMatrix(inner, colsB);

                            AlgorithmRegistry.MatrixMul(A, B);

                            double sum = 0;
                            for (int r = 0; r < runs; r++)
                            {
                                var sw = System.Diagnostics.Stopwatch.StartNew();
                                AlgorithmRegistry.MatrixMul(A, B);
                                sw.Stop();
                                sum += sw.Elapsed.TotalMilliseconds;
                            }
                            heat[i, j] = sum / runs;

                            doneCost += (double)rowsA * colsB * inner;
                        }

                        int percent = (int)(100.0 * doneCost / totalCost);
                        int capturedI = i;
                        BeginInvoke(new Action(() =>
                        {
                            progressBar.Value = Math.Min(percent, 100);
                            lblProgress.Text = $"3D: {percent}% (строка {capturedI + 1}/{cells})";
                        }));
                    }

                    BeginInvoke(new Action(() =>
                    {
                        Plot3DMatrixHeatmap(heat, ns, ms);
                        Log("3D-эксперимент (heatmap матриц) завершён.");
                        lblProgress.Text = "Готово.";
                        progressBar.Value = 100;
                        btnRunSeries.Enabled = true;
                        btn3DMatrices.Enabled = true;
                    }));
                }
                catch (Exception ex)
                {
                    BeginInvoke(new Action(() =>
                    {
                        MessageBox.Show("Ошибка: " + ex.Message,
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        btnRunSeries.Enabled = true;
                        btn3DMatrices.Enabled = true;
                    }));
                }
            });
        }

        private void Plot3DMatrixHeatmap(double[,] data, int[] ns, int[] ms)
        {
            var plotForm = new Form
            {
                Text = "Часть II. Умножение матриц: время как функция (n, m)",
                Width = 900,
                Height = 700,
                StartPosition = FormStartPosition.CenterParent
            };

            var formsPlot = new ScottPlot.FormsPlot
            {
                Dock = DockStyle.Fill
            };
            plotForm.Controls.Add(formsPlot);

            var plt = formsPlot.Plot;

            var hm = plt.AddHeatmap(data, lockScales: true);
            hm.Update(data,
                      min: data.Cast<double>().Min(),
                      max: data.Cast<double>().Max());

            var cb = plt.AddColorbar(hm);
            cb.Label = "Время (мс)";

            plt.XLabel($"Столбцы B (m): от {ms[0]} до {ms[ms.Length - 1]}");
            plt.YLabel($"Строки A (n): от {ns[0]} до {ns[ns.Length - 1]}");
            plt.Title($"Время умножения A (n × {data.GetLength(0)}) на B (m × n)");

            formsPlot.Refresh();
            plotForm.ShowDialog(this);
        }

        // ============================================================
        // ==============   ЭТАП 1: АВТОПОДБОР N_MAX   ================
        // ============================================================

        private void BtnAutoNMax_Click(object sender, EventArgs e)
        {
            if (chkComparePow != null && chkComparePow.Checked)
            {
                MessageBox.Show(
                    "Для алгоритмов возведения в степень используется фиксированный " +
                    "диапазон n = 1..1000 (считаются шаги, а не время).",
                    "N_max", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (cmbAlgoRegistry.SelectedIndex < 0) return;
            var algo = AlgorithmRegistry.All[cmbAlgoRegistry.SelectedIndex];

            const double limitSeconds = 5.0;
            int n = 10;
            int lastGoodN = 10;
            double lastGoodMs = 0;

            btnRunSeries.Enabled = false;
            btnAutoNMax.Enabled = false;
            progressBar.Value = 0;
            lblProgress.Text = "Подбор N_max...";

            Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        var input = algo.GenerateInput(n);
                        try { algo.Run(CloneInput(input)); } catch { }

                        var sw = System.Diagnostics.Stopwatch.StartNew();
                        algo.Run(CloneInput(input));
                        sw.Stop();

                        double ms = sw.Elapsed.TotalMilliseconds;
                        double sec = ms / 1000.0;

                        int capturedN = n;
                        double capturedMs = ms;
                        BeginInvoke(new Action(() =>
                        {
                            lblProgress.Text = $"n = {capturedN}, время = {capturedMs:F1} мс " +
                                               $"({capturedMs / 1000:F2} с)";
                            Log($"N_max-подбор: n = {capturedN}, {capturedMs:F1} мс");
                        }));

                        if (sec > limitSeconds) break;
                        lastGoodN = n;
                        lastGoodMs = ms;

                        if (n >= 100000) break;

                        int nextN = (int)Math.Max(n + 1, Math.Round(n * 1.5));
                        n = nextN;
                    }

                    BeginInvoke(new Action(() =>
                    {
                        numNTo.Value = Math.Min(lastGoodN, numNTo.Maximum);
                        numNFrom.Value = Math.Min(10, numNFrom.Maximum);

                        Log($"N_max для {algo.Display}: {lastGoodN} " +
                            $"(последний прогон {lastGoodMs:F1} мс)");

                        lblProgress.Text = $"N_max = {lastGoodN}. Установлено в «n до».";

                        progressBar.Value = 100;
                        btnRunSeries.Enabled = true;
                        btnAutoNMax.Enabled = true;
                    }));
                }
                catch (Exception ex)
                {
                    BeginInvoke(new Action(() =>
                    {
                        MessageBox.Show("Ошибка подбора N_max: " + ex.Message,
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        btnRunSeries.Enabled = true;
                        btnAutoNMax.Enabled = true;
                    }));
                }
            });
        }

        // ============================================================
        // ==============   ЭКСПОРТ ОТЧЁТА   ==========================
        // ============================================================

        private void BtnExportReport_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV (*.csv)|*.csv|Текстовый (*.txt)|*.txt";
                sfd.FileName = $"report_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    var lines = new List<string>();

                    lines.Add("=== СВОДКА ПО АППРОКСИМАЦИЯМ ===");
                    lines.Add("Algorithm;Display;Model;C;MSE;Timestamp");

                    var approximations = Database.GetAllApproximations();
                    foreach (var a in approximations)
                    {
                        var info = AlgorithmRegistry.All.Find(x => x.Key == a.Algorithm);
                        string display = info != null ? info.Display : a.Algorithm;

                        lines.Add(string.Join(";", new[]
                        {
                            a.Algorithm,
                            display,
                            a.Model,
                            a.C.ToString("G6"),
                            a.MSE.ToString("G6"),
                            a.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")
                        }));
                    }

                    lines.Add("");
                    lines.Add("=== ЗАМЕРЫ (медиана по n) ===");
                    lines.Add("Algorithm;N;MedianMs");

                    var keys = Database.GetAllAlgorithmKeys();
                    foreach (var key in keys)
                    {
                        var data = Database.GetAverages(key);
                        foreach (var p in data)
                        {
                            lines.Add($"{key};{p.N};{p.Avg.ToString("G6")}");
                        }
                    }

                    File.WriteAllLines(sfd.FileName, lines);

                    Log($"Отчёт сохранён: {sfd.FileName}");

                    MessageBox.Show(
                        $"Отчёт сохранён:\n{sfd.FileName}\n\n" +
                        $"Аппроксимаций: {approximations.Count}\n" +
                        $"Алгоритмов с замерами: {keys.Count}",
                        "Экспорт завершён",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка экспорта: " + ex.Message,
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ============================================================
        // ==============   ВСЕ ЗАМЕРЫ В БД   =========================
        // ============================================================

        private void BtnShowAll_Click(object sender, EventArgs e)
        {
            var keys = Database.GetAllAlgorithmKeys();
            if (keys.Count == 0)
            {
                MessageBox.Show("В БД пока нет ни одного замера.",
                    "Пусто", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var approximations = Database.GetAllApproximations();
            var approxByKey = new Dictionary<string, Approximation>();
            foreach (var a in approximations)
                if (!approxByKey.ContainsKey(a.Algorithm))
                    approxByKey[a.Algorithm] = a;

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Алгоритм;Точек;N от;N до;C;MSE;Модель");
            sb.AppendLine(new string('-', 100));

            foreach (var key in keys)
            {
                var data = Database.GetAverages(key);
                if (data.Count == 0) continue;

                var info = AlgorithmRegistry.All.Find(x => x.Key == key);
                string display = info != null ? info.Display : key;

                string c = "", mse = "", model = "";
                if (approxByKey.ContainsKey(key))
                {
                    var ap = approxByKey[key];
                    c = ap.C.ToString("G4");
                    mse = ap.MSE.ToString("G4");
                    model = ap.Model;
                }

                sb.AppendLine($"{display};{data.Count};{data[0].N};{data[data.Count - 1].N};{c};{mse};{model}");
            }

            var form = new Form
            {
                Text = "Все замеры в БД",
                Width = 900,
                Height = 500,
                StartPosition = FormStartPosition.CenterParent
            };

            var tb = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                Font = new Font("Consolas", 9),
                WordWrap = false,
                Text = sb.ToString()
            };
            form.Controls.Add(tb);

            form.ShowDialog(this);
        }

        // ============================================================
        // =========   ЗАПУСК ВСЕХ АЛГОРИТМОВ И СОХРАНЕНИЕ   ==========
        // ============================================================

        private void BtnRunAll_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV (*.csv)|*.csv";
                sfd.FileName = $"all_results_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                if (sfd.ShowDialog() != DialogResult.OK) return;

                string targetFile = sfd.FileName;

                btnRunSeries.Enabled = false;
                btnRunAll.Enabled = false;
                btnAutoNMax.Enabled = false;
                btnExportReport.Enabled = false;
                btnClearDb.Enabled = false;
                btnClearAllDb.Enabled = false;
                btnCancelRunAll.Enabled = true;
                cancelRunAll = false;

                progressBar.Value = 0;
                lblProgress.Text = "Запуск всех алгоритмов...";

                Task.Run(() =>
                {
                    try
                    {
                        var summary = new List<Approximation>();
                        var allData = new Dictionary<string, List<(int N, double T)>>();

                        int total = AlgorithmRegistry.All.Count;
                        int index = 0;

                        foreach (var algo in AlgorithmRegistry.All)
                        {
                            if (cancelRunAll)
                            {
                                BeginInvoke(new Action(() =>
                                    Log("Отменено пользователем.")));
                                break;
                            }

                            index++;
                            int capturedIndex = index;
                            string capturedKey = algo.Key;
                            string capturedDisplay = algo.Display;

                            BeginInvoke(new Action(() =>
                            {
                                int pct = (int)(100.0 * (capturedIndex - 1) / total);
                                progressBar.Value = Math.Min(pct, 100);
                                lblProgress.Text =
                                    $"({capturedIndex}/{total}) {capturedDisplay}";
                                Log($"--- Запуск: {capturedDisplay} ---");
                            }));

                            if (Database.HasMeasurements(algo.Key))
                            {
                                var cached = Database.GetAverages(algo.Key);
                                if (cached.Count > 0)
                                {
                                    allData[algo.Key] = cached;
                                    var fitCached = Approximator.Fit(cached, algo.TheoreticalModel);
                                    summary.Add(new Approximation
                                    {
                                        Algorithm = algo.Key,
                                        Model = algo.TheoreticalModel,
                                        C = fitCached.C,
                                        MSE = fitCached.MSE,
                                        Timestamp = DateTime.Now
                                    });
                                    BeginInvoke(new Action(() =>
                                        Log($"{capturedDisplay}: из кэша ({cached.Count} точек)")));
                                    continue;
                                }
                            }

                            var range = GetDefaultRange(algo);
                            List<(int N, double T)> data;
                            try
                            {
                                data = RunSeriesForAll(algo,
                                    range.nFrom, range.nTo, range.step, range.runs);
                            }
                            catch (Exception ex)
                            {
                                BeginInvoke(new Action(() =>
                                    Log($"{capturedDisplay}: ОШИБКА — {ex.Message}")));
                                continue;
                            }

                            if (data == null || data.Count == 0)
                            {
                                BeginInvoke(new Action(() =>
                                    Log($"{capturedDisplay}: нет данных")));
                                continue;
                            }

                            allData[algo.Key] = data;

                            var fit = Approximator.Fit(data, algo.TheoreticalModel);

                            Database.SaveApproximation(new Approximation
                            {
                                Algorithm = algo.Key,
                                Model = algo.TheoreticalModel,
                                C = fit.C,
                                MSE = fit.MSE,
                                Timestamp = DateTime.Now
                            });

                            summary.Add(new Approximation
                            {
                                Algorithm = algo.Key,
                                Model = algo.TheoreticalModel,
                                C = fit.C,
                                MSE = fit.MSE,
                                Timestamp = DateTime.Now
                            });

                            BeginInvoke(new Action(() =>
                                Log($"{capturedDisplay}: C = {fit.C:G4}, MSE = {fit.MSE:G4}")));
                        }

                        SaveCombinedReport(targetFile, summary, allData);

                        BeginInvoke(new Action(() =>
                        {
                            progressBar.Value = 100;
                            lblProgress.Text = $"Готово. Сохранено: {targetFile}";
                            Log($"Отчёт сохранён: {targetFile}");

                            MessageBox.Show(
                                $"Все алгоритмы обработаны.\n\n" +
                                $"Отчёт сохранён:\n{targetFile}\n\n" +
                                $"Записей в сводке: {summary.Count}",
                                "Готово",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }));
                    }
                    catch (Exception ex)
                    {
                        BeginInvoke(new Action(() =>
                            MessageBox.Show("Ошибка: " + ex.Message,
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error)));
                    }
                    finally
                    {
                        BeginInvoke(new Action(() =>
                        {
                            btnRunSeries.Enabled = true;
                            btnRunAll.Enabled = true;
                            btnAutoNMax.Enabled = true;
                            btnExportReport.Enabled = true;
                            btnClearDb.Enabled = true;
                            btnClearAllDb.Enabled = true;
                            btnCancelRunAll.Enabled = false;
                        }));
                    }
                });
            }
        }

        private (int nFrom, int nTo, int step, int runs) GetDefaultRange(
            AlgorithmRegistry.AlgorithmInfo algo)
        {
            if (algo.Key.StartsWith("Pow"))
                return (1, 1000, 1, 5);

            if (algo.Key == "BubbleSort")
                return (100, 3000, 100, 5);

            if (algo.Key == "MergeSort" || algo.Key == "QuickSort" ||
                algo.Key == "Timsort" || algo.Key == "TimsortReal")
                return (100, 5000, 100, 5);

            if (algo.Key == "MatrixMul")
                return (10, 100, 10, 3);

            if (algo.Key == "Strassen")
                return (4, 64, 4, 3);

            if (algo.Key == "Dijkstra")
                return (10, 300, 10, 5);

            if (algo.Key == "Const")
                return (10, 1000, 10, 5);

            return (10, 500, 10, 5);
        }

        private List<(int N, double T)> RunSeriesForAll(
            AlgorithmRegistry.AlgorithmInfo algo,
            int nFrom, int nTo, int step, int runs)
        {
            var byN = new Dictionary<int, List<double>>();

            for (int n = nFrom; n <= nTo; n += step)
            {
                if (cancelRunAll)
                    return new List<(int, double)>();

                var input = algo.GenerateInput(n);
                try { algo.Run(CloneInput(input)); } catch { }

                for (int run = 1; run <= runs; run++)
                {
                    double value;
                    if (algo.Key.StartsWith("Pow"))
                        value = MeasureStepsFor(algo.Key, n);
                    else
                    {
                        var sw = System.Diagnostics.Stopwatch.StartNew();
                        algo.Run(CloneInput(input));
                        sw.Stop();
                        value = sw.Elapsed.TotalMilliseconds;
                    }

                    var m = new Measurement
                    {
                        Algorithm = algo.Key,
                        N = n,
                        Value = value,
                        RunIndex = run,
                        Timestamp = DateTime.Now
                    };
                    Database.SaveMeasurement(m);

                    if (!byN.ContainsKey(n)) byN[n] = new List<double>();
                    byN[n].Add(value);
                }
            }

            return MedianByN(byN);
        }

        private void SaveCombinedReport(
            string path,
            List<Approximation> summary,
            Dictionary<string, List<(int N, double T)>> allData)
        {
            var lines = new List<string>();

            lines.Add("=== СВОДКА ПО ВСЕМ АЛГОРИТМАМ ===");
            lines.Add("Algorithm;Display;Model;C;MSE;Timestamp");

            foreach (var a in summary)
            {
                var info = AlgorithmRegistry.All.Find(x => x.Key == a.Algorithm);
                string display = info != null ? info.Display : a.Algorithm;

                lines.Add(string.Join(";", new[]
                {
                    a.Algorithm,
                    display,
                    a.Model,
                    a.C.ToString("G6"),
                    a.MSE.ToString("G6"),
                    a.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")
                }));
            }

            lines.Add("");
            lines.Add("=== ЗАМЕРЫ (медиана по n) ===");
            lines.Add("Algorithm;Display;N;MedianMs");

            foreach (var kv in allData)
            {
                var info = AlgorithmRegistry.All.Find(x => x.Key == kv.Key);
                string display = info != null ? info.Display : kv.Key;

                foreach (var p in kv.Value)
                {
                    lines.Add($"{kv.Key};{display};{p.N};{p.T.ToString("G6")}");
                }
            }

            File.WriteAllLines(path, lines);
        }

        // ============================================================
        // ==============   ВСПОМОГАТЕЛЬНОЕ   =========================
        // ============================================================

        private void Log(string message)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.BeginInvoke(new Action(() => Log(message)));
                return;
            }
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
        }
    }
}