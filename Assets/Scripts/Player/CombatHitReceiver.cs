using UnityEngine;
using UnityEngine.Rendering;
public struct Conexions
{
    public WeaponType type;
    public float counter; 
}

public class CombatHitReceiver : MonoBehaviour
{
    public bool isBlocking;
    public HealthController healthController;

    private Conexions conexions;

    public void OnHit(HitInfo info)
    {
        switch(info.type)
        {
            case WeaponType.Fist:
                conexions.type = WeaponType.Fist;
                if(isBlocking)
                {
                    healthController.TakeDamague(info.damague/2);
                    if (conexions.type == WeaponType.None || info.type == conexions.type)
                    {
                        conexions.counter += 0.5f;
                    }
                }
                else
                {
                    healthController.TakeDamague(info.damague);
                    if (conexions.type == WeaponType.None|| info.type == conexions.type)
                    {
                        conexions.counter++;
                    }

                }

                break;

            case WeaponType.Bat:
                conexions.type = WeaponType.Bat;
                if (isBlocking)
                {
                    healthController.TakeDamague(info.damague);
                    if (conexions.type == WeaponType.None || info.type == conexions.type)
                    {
                        conexions.counter += 0.5f;
                    }
                }
                else
                {
                    if (conexions.type == WeaponType.None || info.type == conexions.type)
                    {
                        conexions.counter++;
                    }

                }

                break;
        }
    }
}
