﻿using System;
using Arch.LowLevel;
using Retard.Core.ViewModels.Input;

namespace Retard.Engine.Models.Input
{
    /// <summary>
    /// Contient l'event retournant la valeur
    /// de l'action
    /// </summary>
    public struct InputActionVector1DEvent
    {
        #region Propriétés

        /// <summary>
        /// Appelé quand l'action est en cours
        /// </summary>
        public Action<float> Performed
        {
            get => InputManager.ActionVector1DResources.Get(in this._performed);
            set => InputManager.ActionVector1DResources.Get(in this._performed) = value;
        }

        #endregion

        #region Evénements

        /// <summary>
        /// Appelé quand l'action est en cours
        /// </summary>
        private Handle<Action<float>> _performed;

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="performed">Handle de l'action en cours de l'event</param>
        public InputActionVector1DEvent(Handle<Action<float>> performed)
        {
            this._performed = performed;
        }

        #endregion
    }
}
