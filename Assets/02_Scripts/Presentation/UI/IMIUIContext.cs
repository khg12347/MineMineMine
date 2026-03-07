using UnityEngine;

namespace MI.Presentation.UI
{
    // ── UI 컨텍스트 계약 ────────────────────────────────────────────────

    public interface IMIUIContext
    {
        IMIHUD HUD { get; }
    }

    public interface IMIHUD
    {
    }

}
