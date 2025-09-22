using UnityEngine;
using Unity.Cinemachine;
public class PlayerAnimation : MonoBehaviour
{
     PlayerCombat _playerCombat;


    public Animator anim;
    float timer;
    bool startTime;
    public int attack;
    [SerializeField] float shakeForceAttack;
    [SerializeField] Vector3 velocityAttack;

    public void Initialize(PlayerCombat playerCombat)
    {
       _playerCombat = playerCombat;
       _playerCombat.OnAttack += Attack;
    }


    private void OnDisable()
    {
        _playerCombat.OnAttack -= Attack;
    }

    void Attack(int side)
    {
        if(side == 1)
        {
            anim.Play("armRight");
        }
        else
        {
            anim.Play("armLeft");
        }
           
    }

    void Shake()
    {
        CameraShake.cameraShakeInstance.Shake(shakeForceAttack, velocityAttack);

    }
}
