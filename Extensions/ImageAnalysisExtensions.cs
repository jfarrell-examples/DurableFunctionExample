using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace Farrellsoft.Example.FileApproval.Extensions
{
    public static class ImageAnalysisExtensions
    {
        public static List<OcrResult> AsResultList(this ImageAnalysis analysisResult, string fileId)
        {
            var returnList = new List<OcrResult>();
            returnList.AddRange(analysisResult.Adult.AsOcrPairs(fileId, OcrType.ComputerVision));
            returnList.AddRange(analysisResult.Color.AsOcrPairs(fileId, OcrType.ComputerVision));
            returnList.AddRange(analysisResult.ImageType.AsOcrPairs(fileId, OcrType.ComputerVision));
            returnList.AddRange(analysisResult.Description.Captions.FirstOrDefault()?.AsOcrPairs(fileId, OcrType.ComputerVision));
            //returnList.AddRange(analysisResult.Brands.AsOcrPairs(fileId, OcrType.ComputerVision));
            //returnList.AddRange(analysisResult.Faces.AsOcrPairs(fileId, OcrType.ComputerVision));
            
            return returnList;
        }

        static IEnumerable<OcrResult> AsOcrPairs(this object obj, string fileId, OcrType ocrType)
        {
            foreach (var propertyInfo in obj.GetType().GetProperties())
            {
                if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType) == false || propertyInfo.PropertyType == typeof(string))
                {
                    yield return new OcrResult(fileId)
                    {
                        KeyName = propertyInfo.Name,
                        OcrValue = propertyInfo.GetValue(obj).ToString(),
                        OcrType = ocrType
                    };
                }
            }
        }
    }
}