namespace MI.Data.Config
{
    /// <summary>
    /// 곡괭이 관련 Config ScriptableObject 묶음. VContainer 등록 단위.
    /// </summary>
    public interface IMIPickaxeDataRegistry
    {
        MIPickaxeCraftConfig CraftConfig { get; }
        MIPickaxeSpecDataTable SpecDataTable { get; }
    }
}
