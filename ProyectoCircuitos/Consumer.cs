using ProyectoCircuitos.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoCircuitos
{
    public class Consumer
    {
        private string BaseUrl;

        public Consumer()
        {
            BaseUrl = "http://192.168.1.25/";
        }

        private bool EncenderLuces(RestRequest request)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(BaseUrl);
            request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };

            var response = client.Execute(request);
            return true;
        }

        public bool Encender(string accion, AreaModel area)
        {
            var request = new RestRequest();

            request.Resource = $"{area.Nombre.ToUpper()}=" + "{accion}";
            request.AddParameter("accion", accion, ParameterType.UrlSegment);

            return EncenderLuces(request);
        }
    }
}
