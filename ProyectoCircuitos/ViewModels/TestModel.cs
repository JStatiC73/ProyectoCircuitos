using Microsoft.CognitiveServices.Speech;
using ProyectoCircuitos.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ProyectoCircuitos.ViewModels
{
    public class TestModel : NotificationObject
    {
        #region Comandos
        public Command LucesSalaCommand { get; set; }
        public Command LucesCocinaCommand { get; set; }
        public Command LucesCorredorCommand { get; set; }
        public Command TodasLucesCommand { get; set; }
        public Command VozCommand { get; set; }
        #endregion
        SpeechRecognizer recognizer;
        IMicrophoneService micService;
        bool isTranscribing;
        string textoTraducido;

        private AreaModel areaSala;
        private AreaModel areaCocina;
        private AreaModel areaCorredor;
        private AreaModel todasAreas;

        public AreaModel AreaSala
        {
            get => areaSala;
            set
            {
                areaSala = value;
                OnPropertyChanged();
            }
        }

        public AreaModel AreaCocina
        {
            get => areaCocina;
            set
            {
                areaCocina = value;
                OnPropertyChanged();
            }
        }

        public AreaModel AreaCorredor
        {
            get => areaCorredor;
            set
            {
                areaCorredor = value;
                OnPropertyChanged();
            }
        }

        public AreaModel TodasAreas
        {
            get => todasAreas;
            set
            {
                todasAreas = value;
                OnPropertyChanged();
            }
        }

        public bool IsTranscribing
        {
            get => isTranscribing;
            set
            {
                isTranscribing = value;
                OnPropertyChanged();
            }
        }

        public string TextoTraducido
        {
            get => textoTraducido;
            set
            {
                textoTraducido = value;
                OnPropertyChanged();
            }
        }
        public TestModel()
        {
            AreaSala = new AreaModel("1", "Sala");
            AreaCocina = new AreaModel("2", "Cocina");
            AreaCorredor = new AreaModel("3", "Corredor");
            TodasAreas = new AreaModel("4", "Todas");
            LucesSalaCommand = new Command(async () => await EncenderLuces(AreaSala));
            LucesCocinaCommand = new Command(async () => await EncenderLuces(AreaCocina));
            LucesCorredorCommand = new Command(async () => await EncenderLuces(AreaCorredor));
            TodasLucesCommand = new Command(async () => await EncenderLuces(TodasAreas));
            VozCommand = new Command(async () => await TraducirVoz());
            micService = DependencyService.Resolve<IMicrophoneService>();
        }

        public async Task EncenderLuces(AreaModel area, string accionLuz = "")
        {

            await Task.Run(() =>
            {
                Consumer consumer = new Consumer();
                string accion = string.IsNullOrEmpty(accionLuz) ? (area.Iluminado ? "OFF" : "ON") : accionLuz;
                consumer.Encender(accion, area);

                if(area.Nombre == "Todas")
                {
                    TodasLuces(accion);
                }
                else if (accion == "ON")
                    area.Iluminado = true;
                else 
                    area.Iluminado = false;

                if(AreaCocina.Iluminado || AreaSala.Iluminado || AreaCorredor.Iluminado)
                {
                    TodasAreas.Iluminado = true;
                }
                else if(!AreaCocina.Iluminado && !AreaSala.Iluminado && !AreaCorredor.Iluminado)
                {
                    TodasAreas.Iluminado = false;
                }
            });
        }

        public void TodasLuces(string accion)
        {
            if(accion == "ON")
            {
                AreaCocina.Iluminado = true;
                AreaSala.Iluminado = true;
                AreaCorredor.Iluminado = true;
            }
            else
            {
                AreaCocina.Iluminado = false;
                AreaSala.Iluminado = false;
                AreaCorredor.Iluminado = false;
            }
        }

        public async Task TraducirVoz()
        {
            bool isMicEnabled = await micService.GetPermissionAsync();

            // EARLY OUT: make sure mic is accessible
            if (!isMicEnabled)
            {
                await Application.Current.MainPage.DisplayAlert("Aviso!", "Por favor conceda permisos para utilizar el microfono", "Aceptar");
                return;
            }

            if (recognizer == null)
            {
                var config = SpeechConfig.FromSubscription(Constants.CognitiveServicesApiKey, Constants.CognitiveServicesRegion);
                recognizer = new SpeechRecognizer(config, "es-GT");
                recognizer.Recognized += async (obj, args) =>
                {
                    await UpdateTranscription(args.Result.Text);
                };
            }

            // if already transcribing, stop speech recognizer
            if (IsTranscribing)
            {
                try
                {
                    await recognizer.StopContinuousRecognitionAsync();
                }
                catch (Exception ex)
                {
                    await UpdateTranscription(ex.Message);
                }
                IsTranscribing = false;
            }

            // if not transcribing, start speech recognizer
            else
            {
                try
                {
                    await recognizer.StartContinuousRecognitionAsync();
                }
                catch (Exception ex)
                {
                    await UpdateTranscription(ex.Message);
                }
                IsTranscribing = true;
            }
        }

        async Task UpdateTranscription(string newText)
        {
            string textoComun = newText.ToLower();
            if(textoComun.Contains("cocina"))
            {
                if(textoComun.Contains("iluminar") || textoComun.Contains("encender") || textoComun.Contains("prender"))
                {
                    await EncenderLuces(AreaCocina, "ON");
                }
                else
                {
                    await EncenderLuces(AreaCocina, "OFF");
                }
            }
            else if (textoComun.Contains("sala"))
            {
                if (textoComun.Contains("iluminar") || textoComun.Contains("encender") || textoComun.Contains("prender"))
                {
                    await EncenderLuces(AreaSala, "ON");
                }
                else
                {
                    await EncenderLuces(AreaSala, "OFF");
                }
            }
            else if(textoComun.Contains("corredor"))
            {
                if (textoComun.Contains("iluminar") || textoComun.Contains("encender") || textoComun.Contains("prender"))
                {
                    await EncenderLuces(AreaCorredor, "ON");
                }
                else
                {
                    await EncenderLuces(AreaCorredor, "OFF");
                }
            }
            else if(textoComun.Contains("todas"))
            {
                if (textoComun.Contains("iluminar") || textoComun.Contains("encender") || textoComun.Contains("prender"))
                {
                    await EncenderLuces(TodasAreas, "ON");
                }
                else
                {
                    await EncenderLuces(TodasAreas, "OFF");
                }
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (!string.IsNullOrWhiteSpace(newText))
                    {
                        TextoTraducido += $"No se reconoce el comando\n";
                    }
                });
            }
        }
    }
}
