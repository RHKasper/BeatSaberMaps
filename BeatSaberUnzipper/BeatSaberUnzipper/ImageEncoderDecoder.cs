using System;
using Aspose.Imaging;

namespace BeatSaberUnzipper
{
	public static class ImageEncoderDecoder
	{
		public static string Base64Encode(string path) 
		{
			byte[] imageArray = System.IO.File.ReadAllBytes(path);
			string base64ImageRepresentation = Convert.ToBase64String(imageArray);
			return base64ImageRepresentation;
		}
		
		public static string Base64Decode(string base64EncodedData) {
			var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
			return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
		}

		public static void ResizeImage()
		{
			// Load image
			using Image image = Image.Load("aspose-logo.jpg");
			// Resize image and save the resized image
			image.Resize(300, 300);
			image.Save("SimpleResizing_out.jpg");
			image.Dispose();
		}
	}
}