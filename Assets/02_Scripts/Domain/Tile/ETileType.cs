using Sirenix.OdinInspector;

namespace MI.Domain.Tile
{
    public enum ETileType
    {
        None = 0,
        [LabelText("흙 1")]
        Soil1 = 10,       
        [LabelText("흙 2")]
        Soil2 = 11,      
        [LabelText("흙 3")]
        Soil3 = 12,      
        [LabelText("나무 1")]
        Wood1 = 20,      
        [LabelText("나무 2")]
        Wood2 = 21,      
        [LabelText("나무 3")]
        Wood3 = 22,
        [LabelText("돌 1")]
        Stone1 = 30,
        [LabelText("돌 2")]
        Stone2 = 31,
        [LabelText("돌 3")]
        Stone3 = 32,

    }
}
