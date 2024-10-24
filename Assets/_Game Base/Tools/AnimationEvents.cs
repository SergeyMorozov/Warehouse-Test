using GAME;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    private PlayerObject _player;

    private void Start()
    {
        _player = GetComponentInParent<PlayerObject>();
    }

    public void AnimAttack()
    {
        // PlayerSystem.Events.AnimAttack?.Invoke(_player);
    }

    public void AnimShield()
    {
        // PlayerSystem.Events.AnimShield?.Invoke(_player);
    }

    public void AnimHealth()
    {
        // PlayerSystem.Events.AnimHealth?.Invoke(_player);
    }

    public void AnimFireball()
    {
        // PlayerSystem.Events.AnimFireball?.Invoke(_player);
    }
}
