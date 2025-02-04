﻿using System;
using System.Runtime.CompilerServices;
using Arch.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Retard.App.Models;
using Retard.App.ViewModels;
using Retard.Cameras.ViewModels;
using Retard.Engine.Models.Assets;
using Retard.Input.Models.Assets;
using Retard.Input.Models.DTOs;
using Retard.Input.ViewModels;
using Retard.SceneManagement.ViewModels;

namespace Retard.Engine.ViewModels
{
    /// <summary>
    /// Chargée d'initialiser et màj tous les systèmes nécessaires
    /// au fonctionnement du jeu
    /// </summary>
    public abstract class BaseEngine : IDisposable
    {
        #region Variables d'instance

        /// <summary>
        /// Pour charger les ressources du jeu
        /// </summary>
        protected readonly ContentManager _content;

        /// <summary>
        /// Pour afficher les sprites à l'écran
        /// </summary>
        protected readonly SpriteBatch _spriteBatch;

        /// <summary>
        /// Le monde contenant les entités
        /// </summary>
        protected readonly World _world;

        /// <summary>
        /// Gère les paramètres de la fenêtre de jeu
        /// </summary>
        protected readonly AppViewport _appViewport;

        /// <summary>
        /// Gère les paramètres de la fenêtre de jeu
        /// </summary>
        protected readonly AppPerformance _appPerformance;

        /// <summary>
        /// L'instance du jeu
        /// </summary>
        protected readonly Game _game;

        /// <summary>
        /// Le viewport par défaut du jeu (par défaut aux dimensions de la fenêtre)
        /// </summary>
        protected Viewport _defaultViewport;

        /// <summary>
        /// TRUE si l'objet a été disposé
        /// </summary>
        private bool disposedValue;

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="game">Le jeu</param>
        /// <param name="graphicsDeviceManager">Configurateur des paramètres de la fenêtre du jeu</param>
        public BaseEngine(Game game, GraphicsDeviceManager graphicsDeviceManager)
        {
            // Initialise les components

            this._content = game.Content;
            this._spriteBatch = new SpriteBatch(game.GraphicsDevice);
            this._world = World.Create();
            this._game = game;


            // Initialise les managers

            CreateDefaultConfigFiles();
            IInputScheme[] inputSchemes = GetInputSchemes();
            int nbMaxControllers = GetNbMaxControllers(inputSchemes);
            InputConfigDTO inputConfig = GetInputConfig();
            WindowSettings ws = GetWindowSettings();

            InputManager.Instance.InitializeInputSchemes(inputSchemes);
            InputManager.Instance.InitializeSystems(nbMaxControllers);
            InputManager.Instance.RegisterInputActions(_world, nbMaxControllers, inputConfig.Actions);

            this._appViewport = new AppViewport(game, graphicsDeviceManager);
            this._appPerformance = new AppPerformance(game);

            this._appViewport.OnWindowResolutionSetEvent += this.OnWindowResolutionSetCallback;
            this._appViewport.SetViewportResolution(ws.WindowSize, ws.FullScreen);
            this._appViewport.SetGameProperties(ws.MouseVisible, ws.AllowUserResizing);
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~BaseEngine()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Charge le contenu externe du jeu
        /// </summary>
        public virtual void LoadContent() { }

        /// <summary>
        /// Màj à chaque frame
        /// </summary>
        /// <param name="game">L'application</param>
        /// <param name="gameTime">Le temps écoulé depuis le début du jeu</param>
        public void Update(Game game, GameTime gameTime)
        {
            if (SceneManager.Instance.IsEmpty)
            {
                game.Exit();
            }

            InputManager.Instance.Update(this._world);

            SceneManager.Instance.UpdateInput(gameTime);
            SceneManager.Instance.Update(this._world, gameTime);

            CameraManager.Instance.Update(this._world);
        }

        /// <summary>
        /// Màj à chaque frame
        /// </summary>
        public void AfterUpdate()
        {
            // Appelé en dernier pour ne pas écraser le précédent KeyboardState
            // avant les comparaisons

            InputManager.Instance.AfterUpdate();
        }

        /// <summary>
        /// Pour afficher des éléments à l'écran
        /// </summary>
        /// <param name="gameTime">Le temps écoulé depuis le début du jeu</param>
        public void Draw(GraphicsDevice graphicsDevice, GameTime gameTime)
        {
            graphicsDevice.Viewport = this._defaultViewport;
            graphicsDevice.Clear(Color.Black);

            SceneManager.Instance.Draw(this._world, gameTime);
        }

        /// <summary>
        /// Libère les allocations mémoire
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Méthodes protégées

        /// <summary>
        /// Crée les fichiers JSON de config par défaut
        /// spécifiques à ce jeu
        /// </summary>
        protected abstract void CreateDefaultConfigFiles();

        /// <summary>
        /// Obtient les contrôleurs acceptés par le jeu
        /// </summary>
        /// <returns>La liste des contrôleurs acceptés par le jeu</returns>
        protected abstract IInputScheme[] GetInputSchemes();

        /// <summary>
        /// Obtient les données de configuration des entrées
        /// </summary>
        /// <returns>Le fichier de configuration des entrées</returns>
        protected abstract InputConfigDTO GetInputConfig();

        /// <summary>
        /// Obtient les paramètres de la fenêtre
        /// </summary>
        protected abstract WindowSettings GetWindowSettings();

        /// <summary>
        /// Crée les scènes du jeu
        /// </summary>
        /// <param name="gameResources">Les ressources du jeu</param>
        protected abstract void CreateScenes(in GameResources gameResources);

        /// <summary>
        /// Libère les allocations mémoire
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    this._appViewport.OnWindowResolutionSetEvent -= this.OnWindowResolutionSetCallback;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Assigne un nouveau viewport par défaut quand la résolution la fenêtre change
        /// </summary>
        /// <param name="windowResolution">Les dimensions de la fenêtre</param>
        private void OnWindowResolutionSetCallback(object _, Point windowResolution)
        {
            this._defaultViewport = new Viewport(0, 0, windowResolution.X, windowResolution.Y);
        }

        /// <summary>
        /// Récupère le nombre max de contrôleurs évalués par l'InputManager.
        /// Si aucune manette n'est connectée, renvoie toujours 1.
        /// </summary>
        /// <param name="inputSchemes">La liste des types de contrôleurs</param>
        /// <returns>Le nombre max de contrôleurs évalués par l'InputManager (1 par défaut)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetNbMaxControllers(IInputScheme[] inputSchemes)
        {
            for (int i = 0; i < inputSchemes.Length; ++i)
            {
                if (inputSchemes[i] is GamePadInput g)
                {
                    return g.NbMaxGamePads;
                }
            }

            return 1;
        }

        #endregion
    }
}