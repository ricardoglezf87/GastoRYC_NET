using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Sheets.v4;


namespace GARCA.WebReport
{
    public class GoogleSheetsUpdater
    {
        private const string SpreadsheetId = "16w9MH6qYkYJdhN5ELtb3C9PaO3ifA6VghXT40O9HzgI";
        private const string SheetName = "Data";

        private const string jsonKey = @"{
                                          ""type"": ""service_account"",
                                          ""project_id"": ""garca-393321"",
                                          ""private_key_id"": ""613077d663c7b272e80b030e4625b6492529504f"",
                                          ""private_key"": ""-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQC+HBmB80e4P+QP\nYXBgvgQqmXmOdVkikXZkGAPEKUCeMnpvLcrTT96Qa5iwx+CN/QDrlDegmVb1mgjd\nFArR7ElLdHkEUwQVOfYcteIYhSf3g//kBaFdX+LVaWJAnxb29yg5wMRupC8ec83q\ndLxVshN06dnvsLZ5ea85npfDcSIHZWboO1XDap5D3e4KWEdmxM41Zi4rLzoiEnbL\nLjBnTCBmc5txlFHW1u/RVp/mXvGOOeMbOlCrNVDFWUZYjnNbD6HgS893gkB5D2K3\nO5qEcJIQD4DRcisiFQzLfscNc6lqDobl74AhNDwGpaFGvkNWqTmWtpOd+MTZ072+\nJdyZlwrLAgMBAAECggEAPgxRzvYmPF6uAs8crJaYKYc6A2MNmpi7eWPVJtsH+hoj\nl9HsVxfEXoKfrWMvSWiAOmN/3Gzv0u1ZYz69YVKrgXMcZGrr9DmcuDYs+y/KoaB1\njmjACAfZsynYJZtHFYgJmv/tnOb9cgT9+j3rklSywP/qX67ff7pVlIrMob91CdBg\nxnTGLAzKE7Vero2fwMNMmSdgtDHZG2154Knc9pio1TV0RtgIShSewVzBjHlHT76C\nJPWs3VbsspvMjmLG9IOh2E5IlMwEPPlEBn8ZWjdff5BT6/JSI78CtJhGOw9La712\nVlha0HnKvAKqRTU0rs++IsEP7Chxn4VZdZFFzCHwYQKBgQDfzB2DcUMN9iacmqbH\nnG1hrPCm3LC/Z5meCumeyGt2Z1TKD2q6b9HsSUS4l7VHyQXCenrVNyPQfeWIGf2A\nQeYzPZAA3MvSFVNUEa7IqgyuR/j3wtXGICf9jZqIPjM6JW7ISivgwgJn4GYd3aoY\nGgfR7iAIMc6gWfxd6N6px+/x6QKBgQDZdw5sRSVDpziR8iSnVCehbVbkQcDW6o2n\n3wd80Hp2XgZcbTGGVZtfU/E4LHi9XKzpdfnLICZZaXW3RtUOfXLLAc666mnrA8Fo\nMhddlAIpKXzGI7hNqzDLUJ9ALpJ7oXya36naY52ZlFCf31S6CpMIH1KCXCWSxl7L\nfPfI+q7SkwKBgQCxfEh/wOiD/w5aLpBMSDO7PhNQ0j9eXQRIgbELmzkWmxZ0dUuY\ndiwO74CPwMklGDj4JniZ9fPyWpYZnTsdZ6XwR0eYV4NRplYCm095ltsdsLizjZJK\nYaA1hwLiQCoSWDOGpsQrqNGNnjp+Pxixqps/E1HWDeIn65WoyJEmJwmVWQKBgAnq\nL7IoBObsvNuKmrk2UQg/H/MQPGaQEx1QQdsi9jJnXwrfv55MUOUrwpe5D1gy3X9X\nbaghMx5ofIF6rUQX3B/9WRuUjHe2mB7UCdSuZtrRlWrDPifsYjEi39/hP7R7TF8T\nS381g335ESP43rgMu19nbi+bW+sxOCk27WUwaMAZAoGAT1PCglLPCJeu1j81uU1/\nDD6WhxH7JhnWICE5e9B8xtmS38bErbyEcwtDQFvbvhk5jM/EruKy08cGROoCGS6W\nhG73/e6kGfrz4m2amzay2eqG5CpCThI6OaND9EIT40muiBN79qCNmZLe8cRqxgeo\n3gGq3IKU8a4zlC9pmZUswtI=\n-----END PRIVATE KEY-----\n"",
                                          ""client_email"": ""garca-221@garca-393321.iam.gserviceaccount.com"",
                                          ""client_id"": ""100727773485233895854"",
                                          ""auth_uri"": ""https://accounts.google.com/o/oauth2/auth"",
                                          ""token_uri"": ""https://oauth2.googleapis.com/token"",
                                          ""auth_provider_x509_cert_url"": ""https://www.googleapis.com/oauth2/v1/certs"",
                                          ""client_x509_cert_url"": ""https://www.googleapis.com/robot/v1/metadata/x509/garca-221%40garca-393321.iam.gserviceaccount.com"",
                                          ""universe_domain"": ""googleapis.com""
                                        }
                                        ";

        public async Task ActualizarHoja()
        {
            // Cargar las credenciales desde el archivo JSON
            GoogleCredential credential;

            credential = GoogleCredential.FromJson(jsonKey)
                .CreateScoped(SheetsService.Scope.Spreadsheets);

            // Crear el servicio de Google Sheets
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "GARCA"
            });

            // Crear los datos que se actualizarán
            string[] valores = { "Prueba", "Prueba2","85","12.5" };
            var valueRange = new ValueRange
            {
                Values = new List<IList<object>> { valores }
            };

            // Realizar la actualización
            var request = service.Spreadsheets.Values.Update(valueRange, SpreadsheetId, $"{SheetName}!A2");
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            request.Execute();
        }
    }
}
