using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CobimPalette.Common.Converters
{
    public class BitmapConverter
    {
        #region ConvertFromBitmap

        /// <summary>
        /// 리소스에 등록된 
        /// 이미지 파일(.png, .jpg 등등...) -> BitmapSource 
        /// convert 처리 해주는 메서드 
        /// </summary>
        public static BitmapSource ConvertFromBitmap(Bitmap pBitmap)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                pBitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        #endregion ConvertFromBitmap

        #region GetBitmap

        /// <summary>
        /// 리소스에 등록된 
        /// 이미지 파일(.png, .jpg 등등...) -> BitmapImage 
        /// convert 처리 해주는 메서드 
        /// </summary>
        public static BitmapImage GetBitmap(Bitmap bitmap)
        {
            MemoryStream stream = new MemoryStream();   // 메모리 스트림 객체 생성 
            //bitmap.SetResolution(95, 95);
            //System.Windows.MessageBox.Show(bitmap.HorizontalResolution.ToString() + "/" + bitmap.VerticalResolution.ToString());
            //            bitmap.SetResolution(128, 128);

            // 비트맵 이미지 객체 bitmap를 지정된 형식(ImageFormat.Png)의 지정된 메모리 스트림 객체(stream)에 저장
            // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.drawing.image.save?view=windowsdesktop-9.0&viewFallbackFrom=dotnet-plat-ext-8.0
            bitmap.Save(stream, ImageFormat.Png);
            BitmapImage bmp = new BitmapImage();

            // TODO : 메서드 BeginInit 호출하여 비트맵 이미지 객체(bmp) 초기화 시작 구현 (2024.12.04 minjae)
            // 참고 URL - https://learn.microsoft.com/en-us/dotnet/api/system.windows.media.imaging.bitmapimage.begininit?view=windowsdesktop-9.0
            bmp.BeginInit();   // 비트맵 이미지 객체(bmp) 초기화 
            bmp.StreamSource = stream;   // 비트맵 이미지 객체(bmp) 속성 StreamSource 셋팅

            // TODO : 메서드 EndInit 호출하여 비트맵 이미지 객체(bmp) 초기화 완료 구현 (2024.12.04 minjae)
            // 참고 URL - https://learn.microsoft.com/en-us/dotnet/api/system.windows.media.imaging.bitmapimage.endinit?view=windowsdesktop-9.0
            bmp.EndInit();
            //System.Windows.MessageBox.Show(bmp.Width + "/" + bmp.Height);
            return bmp;
        }

        /// <summary>
        /// 리소스에 등록된 
        /// 이미지 파일(.png, .jpg 등등...) -> BitmapImage 
        /// convert 처리 해주는 메서드 
        /// </summary>
        public static BitmapImage GetBitmap(Bitmap bitmap, float resolution)
        {
            MemoryStream stream = new MemoryStream();   // 메모리 스트림 객체 생성
            bitmap.SetResolution(resolution, resolution);   // 비트맵 객체(bitmap) 해상도 설정(SetResolution)
            //System.Windows.MessageBox.Show(bitmap.HorizontalResolution.ToString() + "/" + bitmap.VerticalResolution.ToString());
            //            bitmap.SetResolution(128, 128);
            bitmap.Save(stream, ImageFormat.Png);
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();   // 비트맵 이미지 객체(bmp) 초기화 시작
            bmp.StreamSource = stream;   // 비트맵 이미지 객체(bmp) 속성 StreamSource 셋팅
            bmp.EndInit();   // 비트맵 이미지 객체(bmp) 초기화 완료
            //System.Windows.MessageBox.Show(bmp.Width + "/" + bmp.Height);
            return bmp;
        }

        #endregion GetBitmap

        #region Sample

        #endregion Sample
    }
}
