using LiquidSnake.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace LiquidSnake.Enemies
{
    /// <summary>
    /// Componente auxiliar para poder resetear la m�quina de estados de un enemigo controlado
    /// por un State Graph de visual scripting. 
    /// </summary>
    public class StateGraphEnemy : MonoBehaviour, IResetteable
    {
        //----------------------------------------------------------------------------
        //              Implementaci�n de IResetteable
        //----------------------------------------------------------------------------
        #region M�todos p�blicos e implementaci�n de IResetteable

        public void Reset()
        {
            CustomEvent.Trigger(gameObject, "reset");
        } // Reset
        #endregion
    } // StateGraphEnemy
} // namespace LiquidSnake.Enemies
