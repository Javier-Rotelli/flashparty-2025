using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.Samples.Cameras;
using TGC.MonoGame.Samples.Geometries;
using TGC.MonoGame.TP.Models;

namespace TGC.MonoGame.TP;

/// <summary>
///     Esta es la clase principal del juego.
///     Inicialmente puede ser renombrado o copiado para hacer mas ejemplos chicos, en el caso de copiar para que se
///     ejecute el nuevo ejemplo deben cambiar la clase que ejecuta Program <see cref="Program.Main()" /> linea 10.
/// </summary>
public class TGCGame : Game
{
    public const string ContentFolder3D = "Models/kenney/";
    public const string ContentFolderEffects = "Effects/";
    public const string ContentFolderMusic = "Music/";
    public const string ContentFolderSounds = "Sounds/";
    public const string ContentFolderSpriteFonts = "SpriteFonts/";
    public const string ContentFolderTextures = "Textures/";

    private readonly GraphicsDeviceManager _graphics;

    private Effect _effect;
    private Effect _spectrum_effect;
    private Rails _rail_straight;
    private Quad _quad;
    private bool _isFreeCamera = false;

    private FreeCamera _freeCamera;
    private StaticCamera _fixedCamera;

    private Origin _origin;

    /// <summary>
    ///     Constructor del juego.
    /// </summary>
    public TGCGame()
    {
        // Maneja la configuracion y la administracion del dispositivo grafico.
        _graphics = new GraphicsDeviceManager(this);

        _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
        _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;

        // Para que el juego sea pantalla completa se puede usar Graphics IsFullScreen.
        // Carpeta raiz donde va a estar toda la Media.
        Content.RootDirectory = "Content";
        // Hace que el mouse sea visible.
        IsMouseVisible = true;
    }

    /// <summary>
    ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
    ///     Escribir aqui el codigo de inicializacion: el procesamiento que podemos pre calcular para nuestro juego.
    /// </summary>
    protected override void Initialize()
    {
        // La logica de inicializacion que no depende del contenido se recomienda poner en este metodo.

        // Apago el backface culling.
        // Esto se hace por un problema en el diseno del modelo del logo de la materia.
        // Una vez que empiecen su juego, esto no es mas necesario y lo pueden sacar.
        var rasterizerState = new RasterizerState();
        rasterizerState.CullMode = CullMode.None;
        GraphicsDevice.RasterizerState = rasterizerState;
        // Seria hasta aca.


        _freeCamera = new FreeCamera(GraphicsDevice.Viewport.AspectRatio, Vector3.UnitZ * -150);
        _fixedCamera = new StaticCamera(GraphicsDevice.Viewport.AspectRatio, new Vector3(0f, 75f, -100f), new Vector3(0f, -0.05f, 1f), Vector3.Up);
        base.Initialize();
    }

    /// <summary>
    ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo, despues de Initialize.
    ///     Escribir aqui el codigo de inicializacion: cargar modelos, texturas, estructuras de optimizacion, el procesamiento
    ///     que podemos pre calcular para nuestro juego.
    /// </summary>
    protected override void LoadContent()
    {
        _origin = new Origin(GraphicsDevice);
        // Cargo la textura del modelo.
        var colormap = Content.Load<Texture2D>(ContentFolder3D + "Textures/colormap");

        // Cargo un efecto basico propio declarado en el Content pipeline.
        // En el juego no pueden usar BasicEffect de MG, deben usar siempre efectos propios.
        _effect = Content.Load<Effect>(ContentFolderEffects + "BasicShader");

        _spectrum_effect = Content.Load<Effect>(ContentFolderEffects + "spectrum");

        _rail_straight = new Rails(Content, ContentFolder3D + "coaster-steel-straight", _effect);

        _quad = new Quad(GraphicsDevice, 750, 750);

        // Asigno la textura cargada al efecto.
        _effect.Parameters["Colormap"]?.SetValue(colormap);

        // Luz
        _effect.Parameters["ambientColor"]?.SetValue(Color.Cyan.ToVector3());
        _effect.Parameters["diffuseColor"]?.SetValue(Color.Cyan.ToVector3());
        _effect.Parameters["specularColor"]?.SetValue(new Vector3(1f, 0f, 1f));

        _effect.Parameters["KAmbient"]?.SetValue(0.5f);
        _effect.Parameters["KDiffuse"]?.SetValue(1.0f);
        _effect.Parameters["KSpecular"]?.SetValue(0.8f);
        _effect.Parameters["shininess"]?.SetValue(32.0f);

        base.LoadContent();
    }


    private KeyboardState _previousKeyboardState;

    private bool OnKeyUp(Keys key)
    {
        return !Keyboard.GetState().IsKeyDown(key) && _previousKeyboardState.IsKeyDown(key);
    }

    /// <summary>
    ///     Se llama en cada frame.
    ///     Se debe escribir toda la logica de computo del modelo, asi como tambien verificar entradas del usuario y reacciones
    ///     ante ellas.
    /// </summary>
    protected override void Update(GameTime gameTime)
    {
        // Aca deberiamos poner toda la logica de actualizacion del juego.
        KeyboardState state = Keyboard.GetState();
        // Capturar Input teclado
        if (state.IsKeyDown(Keys.Escape))
        {
            //Salgo del juego.
            Exit();
        }
        if (OnKeyUp(Keys.P))
        {
            _isFreeCamera = !_isFreeCamera;
        }
        if (OnKeyUp(Keys.O))
        {
            GraphicsDevice.RasterizerState = new RasterizerState { CullMode = CullMode.None, FillMode = FillMode.WireFrame };
        }
        else if (OnKeyUp(Keys.I))
        {
            GraphicsDevice.RasterizerState = new RasterizerState { CullMode = CullMode.None, FillMode = FillMode.Solid };
        }

        //UpdateModelPosition(gameTime);
        _freeCamera.Update(gameTime);

        _rail_straight.Update(gameTime, 2000f);
        _quad.Update(gameTime, 2000f);

        base.Update(gameTime);
        _previousKeyboardState = state;
    }

    /// <summary>
    ///     Se llama cada vez que hay que refrescar la pantalla.
    ///     Escribir aqui el codigo referido al renderizado.
    /// </summary>
    protected override void Draw(GameTime gameTime)
    {
        // Aca deberiamos poner toda la logia de renderizado del juego.
        GraphicsDevice.Clear(Color.Black);
        Camera camera = _isFreeCamera ? _freeCamera : _fixedCamera;

        _origin.Draw(camera.View, camera.Projection);
        // Para dibujar le modelo necesitamos pasarle informacion que el efecto esta esperando.
        _effect.Parameters["View"].SetValue(camera.View);
        _effect.Parameters["Projection"].SetValue(camera.Projection);

        _spectrum_effect.Parameters["View"].SetValue(camera.View);
        _spectrum_effect.Parameters["Projection"].SetValue(camera.Projection);



        _effect.Parameters["lightPosition"]?.SetValue(new Vector3(0, 500, 1000));
        _effect.Parameters["eyePosition"]?.SetValue(camera.Position);
        _effect.Parameters["Time"]?.SetValue((float)gameTime.TotalGameTime.TotalSeconds);

        _spectrum_effect.Parameters["Time"]?.SetValue((float)gameTime.TotalGameTime.TotalSeconds);

        _rail_straight.Draw();

        _quad.Draw(_spectrum_effect, gameTime);

    }

    /// <summary>
    ///     Libero los recursos que se cargaron en el juego.
    /// </summary>
    protected override void UnloadContent()
    {
        // Libero los recursos.
        Content.Unload();

        base.UnloadContent();
    }
}