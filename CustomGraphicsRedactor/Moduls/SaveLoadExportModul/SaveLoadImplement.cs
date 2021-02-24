using System;
using System.IO;
using System.Windows;
using System.Xml.Linq;
using Microsoft.Win32;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CustomGraphicsRedactor.Moduls
{
    /// <summary>
    /// Логика сохранения/загрузки/экспорта холста
    /// </summary>
    public static class SaveLoadImplement
    {
        private static Canvas _ctx;

        static SaveLoadImplement()
        {
            _ctx = null;
        }

        /// <summary>
        /// Функция изменения контекста выполнения операций
        /// </summary>
        /// <param name="ctx">Контекст выполнения операций (холст)</param>
        public static void SetContext(Canvas ctx) => _ctx = ctx;

        /// <summary>
        /// Функция определяет операцию взаимодействия и инициирует ее
        /// </summary>
        /// <param name="status">Статус (тип) действия</param>
        public static void SLERoute(ESaveLoadStatus status)
        {
            CurrentSettings.SetCurrentItem();

            switch (status)
            {
                case ESaveLoadStatus.Save:
                    Save();
                    break;
                case ESaveLoadStatus.Load:
                    Load();
                    break;
                case ESaveLoadStatus.Export:
                    Export();
                    break;
            }
        }

        /// <summary>
        /// Функция выполнения операции сохранения холста в свой формат
        /// </summary>
        private static void Save()
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CGR|*.cgr";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (saveFileDialog.ShowDialog() == false) return;

            var xmlParser = new XmlParser(_ctx);

            var xdoc = xmlParser.SerializeXml();

            xdoc.Save(saveFileDialog.FileName);
        }

        /// <summary>
        /// Функция выполнения операции загрузки холста из файла
        /// </summary>
        private static void Load()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CGR|*.cgr";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (openFileDialog.ShowDialog() == false) return;

            CurrentSettings.ClearCancel();

            var xmlParser = new XmlParser(_ctx);

            if (openFileDialog.CheckFileExists) {
                _ctx.Children.Clear();
                xmlParser.DeserializeXml(XDocument.Load(openFileDialog.FileName))
                    .ForEach(c => _ctx.Children.Add(c));
            }

            CurrentSettings.MoveDelegate?.Invoke();
        }

        /// <summary>
        /// Функция выполнения операции экспорта холста в растровое изображение 
        /// </summary>
        private static void Export()
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG|*.png|JPG|*.jpg";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (saveFileDialog.ShowDialog() == false) return;

            var bounds = VisualTreeHelper.GetDescendantBounds(_ctx);
            var dpi = 96d;

            var rtb = new RenderTargetBitmap(
                (int)bounds.Width,
                (int)bounds.Height,
                dpi,
                dpi,
                PixelFormats.Default
            );

            var dv = new DrawingVisual();
            using (var dc = dv.RenderOpen()) {
                var vb = new VisualBrush(_ctx);
                dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            }

            rtb.Render(dv);

            var pngEncoder = new PngBitmapEncoder();
            var jpgEncoder = new JpegBitmapEncoder();

            var filterPng = saveFileDialog.FilterIndex == 0;

            if (filterPng) pngEncoder.Frames.Add(BitmapFrame.Create(rtb));
            else jpgEncoder.Frames.Add(BitmapFrame.Create(rtb));

            try {
                using (var ms = new MemoryStream()) {
                    if (filterPng) pngEncoder.Save(ms);
                    else jpgEncoder.Save(ms);
                    ms.Close();

                    File.WriteAllBytes(saveFileDialog.FileName, ms.ToArray());
                }
            }
            catch (Exception err) {
                MessageBox.Show(
                        err.ToString(),
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
            }
        }
    }
}
