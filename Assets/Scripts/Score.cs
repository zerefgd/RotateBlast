using UnityEngine;

public class Score : MonoBehaviour
{
    public int currentId;

    [SerializeField]
    private Color _activeColor, _inactiveColor;

    [SerializeField]
    private SpriteRenderer _sr;

    private void OnEnable()
    {
        GameManager.UpdateScoreColor += OnTargetSet;
    }

    private void OnDisable()
    {
        GameManager.UpdateScoreColor -= OnTargetSet;
    }

    private void OnTargetSet(int moveId)
    {
        _sr.color = currentId == moveId ? _activeColor : _inactiveColor;
    }
}
