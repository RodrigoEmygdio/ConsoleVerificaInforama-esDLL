using System;
using System.Reflection;
using System.Linq;
using System.IO;
using ConsoleCommon;
using System.Collections.Generic;

namespace VerificaAssembly
{
    class Program
    {
        
        static void Main(string[] args)
        {
            try
            {
                Parametros parametros = new Parametros(args);

                parametros.CheckParams();
                string textoAjuda = parametros.GetHelpIfNeeded();

                if (!string.IsNullOrEmpty(textoAjuda))
                {
                    Console.WriteLine(textoAjuda);
                    Environment.Exit(0);
                }


                if (DicionarioAcao.ContainsKey(parametros.Acao))
                {
                    var acao =  DicionarioAcao[parametros.Acao] as Action<Parametros>;
                    acao(parametros);

                }
                else
                    Console.WriteLine("Nenhuma ação encontrada");
            }
            catch (Exception ex)
            {

                Console.WriteLine("Erro: {0}", ex.Message);
            }    
        }

        static Dictionary<Acao, Action<Parametros>> DicionarioAcao = new Dictionary<Acao, Action<Parametros>>()
        {
            {
                Acao.VersaoFramework,
                (parametro)=> {
                     var dll = Assembly.LoadFile(@parametro.CaminhoDll);
                     Console.WriteLine("Versão framework: {0}", dll.ImageRuntimeVersion);
                }
            }
        };
       
    }

    public class Parametros : ParamsObject
    {
        public Parametros(string[] args) : base(args){}

        [Switch("Acao")]
        [SwitchHelpText("Tipo de informação que deseja extrair")]
        public Acao Acao { get; set; }
        [Switch("CaminhoDll", true)]
        [SwitchHelpText("Caminho abosluto da DLL")]
        public string CaminhoDll { get; set; }
        [HelpText(0)]
        public string Descrcao { get { return "Pesquise por infromaçoes da DLL"; } }
        [HelpText(1, "Exemplo")]
        public string Exemplo { get { return $@"VerificaAssembly.exe /Acao:VersaoFramework /CaminhoDll:\\receitasrv006v\fwkSefaz_NET\BinV2\FwkNegocio.dll {Environment.NewLine}"; } }
        [HelpText(3,"Parametros")]
        public override string SwitchHelp
        {
            get { return base.SwitchHelp; }
        }

        public override Dictionary<Func<bool>, string> GetParamExceptionDictionary()
        {
            return new Dictionary<Func<bool>, string>()
            {
                { ()=> !File.Exists(@CaminhoDll), "Informe um caminho absoluto valido para a DLL!"}
            };
        }
    }
    public enum Acao
    {
        VersaoFramework = 1
    }
    
}
