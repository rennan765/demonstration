namespace _4oito6.Demonstration.Config.Model
{
    public class SwaggerConfig
    {
        public SwaggerConfig(string title, string version, string description, string contactName, string contactEmail, string contactUrl)
        {
            Title = title;
            Version = version;
            Description = description;
            ContactName = contactName;
            ContactEmail = contactEmail;
            ContactUrl = contactUrl;
        }

        public string Title { get; private set; }
        public string Version { get; private set; }
        public string Description { get; private set; }
        public string ContactName { get; private set; }
        public string ContactEmail { get; private set; }
        public string ContactUrl { get; private set; }
    }
}