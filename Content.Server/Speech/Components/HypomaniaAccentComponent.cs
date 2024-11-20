namespace Content.Server.Speech.Components
{
    /// <summary>
    /// Этот компонент отвечает за акцент гипомании, в частности, вызывая смех после каждой фразы с определенным шансом.
    /// <summary>
    [RegisterComponent]
    public sealed partial class HypomaniaAccentComponent : Component
    {
        // Эта переменная определяет вероятность смеха после каждой фразы
        [DataField]
        public float LaughChance = 0.5f;
    }
}
