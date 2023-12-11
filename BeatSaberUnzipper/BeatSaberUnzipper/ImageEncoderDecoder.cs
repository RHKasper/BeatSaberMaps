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
	}
}