﻿using Retard.Core.Models.ValueTypes;

namespace Retard.Core.Models.DTOs.Input
{
    /// <summary>
    /// Représente les données d'un InputAction
    /// </summary>
    public sealed class InputActionDTO
    {
        #region Propriétés

        /// <summary>
        /// L'ID de cette action
        /// </summary>
        public NativeString Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Le type de valeur retournée par une InputAciton
        /// </summary>
        public InputActionReturnValueType ValueType
        {
            get;
            private set;
        }

        /// <summary>
        /// Le type d'action à effectuer lorsqu'on évalue une InputAction donnée
        /// </summary>
        public InputActionTriggerType TriggerType
        {
            get;
            private set;
        }

        /// <summary>
        /// La liste des bindings de cette action
        /// </summary>
        public InputBindingDTO[] Bindings
        {
            get;
            private set;
        }

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="name">L'ID de l'action</param>
        /// <param name="valueType">Le type de valeur retournée par l'action</param>
        /// <param name="triggerType">Le moment durant lequel la valeur doit être évlauée</param>
        /// <param name="bindings">Les entrées liées à cette action</param>
        public InputActionDTO(NativeString name, InputActionReturnValueType valueType, InputActionTriggerType triggerType, params InputBindingDTO[] bindings)
        {
            this.Name = name;
            this.ValueType = valueType;
            this.TriggerType = triggerType;
            this.Bindings = bindings;
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="name">L'ID de l'action</param>
        /// <param name="valueType">Le type de valeur retournée par l'action</param>
        /// <param name="triggerType">Le moment durant lequel la valeur doit être évlauée</param>
        /// <param name="bindings">Les entrées liées à cette action</param>
        public InputActionDTO(string name, InputActionReturnValueType valueType, InputActionTriggerType triggerType, params InputBindingDTO[] bindings)
        {
            this.Name = new NativeString(name);
            this.ValueType = valueType;
            this.TriggerType = triggerType;
            this.Bindings = bindings;
        }

        #endregion
    }
}
