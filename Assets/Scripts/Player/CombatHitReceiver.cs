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
    

    public int batExposeThreshold = 2;

    public int FistExposeThreshold = 3;

    public bool resetCounterOnExpose = true;

    public void OnHit(HitInfo info)
    {
        switch (info.type)
        {
            case WeaponType.Fist:
                conexions.type = WeaponType.Fist;
                if (isBlocking)
                {
                    healthController.TakeDamague(info.damague / 2);
                    if (conexions.type == WeaponType.None || info.type == conexions.type)
                    {
                        conexions.counter += 0.5f;
                    }
                }
                else
                {
                    healthController.TakeDamague(info.damague);
                    if (conexions.type == WeaponType.None || info.type == conexions.type)
                    {
                        conexions.counter++;
                    }

                }

                break;

            case WeaponType.Bat:
                conexions.type = WeaponType.Bat;
                if (isBlocking)
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
                    if (conexions.type == WeaponType.None || info.type == conexions.type)
                    {
                        conexions.counter++;
                    }

                }

                break;
                
                case WeaponType.Crate:
                    conexions.type = WeaponType.Crate;
                    if (isBlocking)
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
                        if (conexions.type == WeaponType.None || info.type == conexions.type)
                        {
                            conexions.counter++;
                        }     
                    }              
                    

                break;

                case WeaponType.Beer:
                    conexions.type = WeaponType.Beer;                    
                    if (healthController != null)
                    {
                        healthController.AddHealth(Mathf.Abs(info.damague));
                    }
                    else
                    {
                        Debug.LogWarning($"CombatHitReceiver: healthController null on {gameObject.name}");
                    }
                break;




        }
        
        var enemyHandler = GetComponentInParent<EnemyStateHandler>();
        if (enemyHandler != null)
        {
            if (conexions.type == WeaponType.Bat && conexions.counter >= batExposeThreshold)
            {                
                var exposedState = enemyHandler.GetExposedState();
                if (exposedState != null)
                {
                    enemyHandler.SetState(exposedState);
                }
                
                if (resetCounterOnExpose) conexions.counter = 0f;
                else conexions.counter = Mathf.Max(0f, conexions.counter - batExposeThreshold);
            }
        }

        
        if (enemyHandler != null)
        {
            if (conexions.type == WeaponType.Fist && conexions.counter >= FistExposeThreshold)
            {                
                var exposedState = enemyHandler.GetExposedState();
                if (exposedState != null)
                {
                    enemyHandler.SetState(exposedState);
                }
                
                if (resetCounterOnExpose) conexions.counter = 0f;
                else conexions.counter = Mathf.Max(0f, conexions.counter - batExposeThreshold);
            }
        }


        



    }
}
