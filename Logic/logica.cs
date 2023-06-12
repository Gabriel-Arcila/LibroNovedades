using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data.OleDb;
using System.Data;
using Newtonsoft.Json;
using LibroNovedades.Models;
using LibroNovedades.ModelsDocIng;
using LibroNovedades.Data.LibroNov;



namespace LibroNovedades.Logic{
    public interface ILogicLibroNov
    {
        Task<bool> CambiosBDLibro(int idCentro,List<LibroNove> listaNovedades,DateTime filtroFechaInicio,DateTime filtroFechaFinal,int filtroLinea,int filtroTipoNovedad,string nombre);
    }

    public class LogicLibroNov : ILogicLibroNov
    {

        public async Task<bool> CambiosBDLibro(int idCentro,List<LibroNove> listaNovedades,DateTime filtroFechaInicio,DateTime filtroFechaFinal,int filtroLinea,int filtroTipoNovedad,string nombre){
            DbNeoContext contex = new DbNeoContext();
            IDataLibroNov dataLibroNov = new DataLibroNov(contex);
            IDataPizarra dataPizarra = new DataPizarra(contex);
            IDataChismoso dataChismoso = new DataChismoso(contex);
            LibroNove? temporal;
            ReunionDium registro = new ReunionDium();
            ReuDium registroNuevo = new ReuDium();
            CambFec ChismosoCambioFecha =  new CambFec();
            CambStat ChismosoCambioEstado = new  CambStat();
            List<CambFec> ListaChismosoCambioFecha  = new List<CambFec>();
            List<CambStat> ListaChismosoCambioEstado  = new List<CambStat>();
            List<ReuDium> listaPizarra = new List<ReuDium>(listaNovedades.Count);
            List<LibroNove> listaNovedades2;
            if(filtroFechaInicio.Date == filtroFechaFinal.Date) {
                listaNovedades2 = await dataLibroNov.ObtenerLibroNovedadesPorFiltro(idCentro,filtroFechaInicio,filtroLinea,filtroTipoNovedad); 
            }else if(filtroFechaInicio.Date < filtroFechaFinal.Date){
                listaNovedades2 = await dataLibroNov.ObtenerLibroNovedadesPorFiltroEntreFechas(idCentro,filtroFechaInicio,filtroFechaFinal,filtroLinea,filtroTipoNovedad);
            }else{
                listaNovedades2 = await dataLibroNov.ObtenerLibroNovedadesPorFiltroEntreFechas(idCentro,filtroFechaFinal,filtroFechaInicio,filtroLinea,filtroTipoNovedad);
            }

            foreach (var item in listaNovedades)
            {
                temporal = listaNovedades2.Find(x=> x.IdlibrNov == item.IdlibrNov);
                if(temporal != null){
                    if((temporal.LnisPizUni != item.LnisPizUni) && (item.LnisPizUni == true)){
                        if(temporal.IdTipoNove == 8 || temporal.IdAreaCar == 2){
                            //* Calidad
                            registroNuevo.Idksf = 3;
                        }else if(temporal.IdTipoNove == 13 || temporal.IdAreaCar == 3){
                            //* Seguridad
                            registroNuevo.Idksf = 5;
                        }else{
                            registroNuevo.Idksf = 1; 
                        }
                        registroNuevo.Rdcentro = temporal.IdLineaNavigation.IdCentroNavigation.Cnom;
                        registroNuevo.Rddiv = temporal.IdLineaNavigation.IdDivisionNavigation.Dnombre;
                        registroNuevo.Rdarea = temporal.IdLineaNavigation.Lnom;
                        registroNuevo.RdcodEq = temporal.IdEquipo;
                        registroNuevo.Rddisc = temporal.Lndiscrepa;
                        registroNuevo.RdfecReu = DateTime.Now;   
                        registroNuevo.RdfecTra = DateTime.Now;
                        registroNuevo.Rdstatus = "Pendiente";
                        registroNuevo.IdResReu =  11;
                        registroNuevo.IdPais = 1;
                        listaPizarra.Add(registroNuevo);

                        ChismosoCambioFecha.IdReuDiaNavigation = registroNuevo;
                        ChismosoCambioFecha.Cffec = DateTime.Now;
                        ChismosoCambioFecha.CffecNew = DateTime.Now;
                        ChismosoCambioFecha.Cfuser = nombre;
                        ListaChismosoCambioFecha.Add(ChismosoCambioFecha);

                        ChismosoCambioEstado.Cbuser = nombre;
                        ChismosoCambioEstado.Cbstat = "Pendiente";
                        ChismosoCambioEstado.Cbfecha =  DateTime.Now;
                        ChismosoCambioEstado.IdReuDiaNavigation =  registroNuevo;
                        ListaChismosoCambioEstado.Add(ChismosoCambioEstado);

                        registroNuevo = new ReuDium();
                        ChismosoCambioFecha = new CambFec();
                        ChismosoCambioEstado = new  CambStat();
                    }
                }else{
                    continue;
                }
            }
            if(listaPizarra.Count > 0){
                //dataPizarra.InsertarRegistros(listaPizarra);
                dataChismoso.InsertarRegistros(ListaChismosoCambioFecha,ListaChismosoCambioEstado);
                return true;
            }else{
                return false;
            }
        }
    }

}