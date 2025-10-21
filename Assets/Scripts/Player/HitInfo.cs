using UnityEngine;

public class HitInfo 
{
    public int damague;
    public Collider col;
    public Vector3 hitpoint;
    public Vector3 normal;
    public IEnemyState enemyState;
    public TagSO typeEnemy;

    public HitInfo(Collider col, Vector3 point, Vector3 normal, int damage, IEnemyState state = null, TagSO tag = null) 
    {
        this.col = col;
        this.hitpoint = point;
        this.normal = normal;
        this.damague = damage;
        this.enemyState = state;
        this.typeEnemy = tag;
    }



    /*public virtual int CalculateDmg()
    {
        if(enemyState is BlockState )
        {
            switch (tag)
            {
                case meleeEnemy:
                    break;

            }
        }
        else if (tag != null)
        {

        }
        else
        {
            return damague;
        }
        return damague;
    } */
}
