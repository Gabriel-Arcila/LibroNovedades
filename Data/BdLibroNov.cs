using System.Linq;
using LibroNovedades.Models;
using LibroNovedades.ModelsDocIng;
using Microsoft.EntityFrameworkCore;

    namespace LibroNovedades.Data.LibroNov
{
    public interface IDataLibroNov
    {
        Task<bool> InsertarRegistro(LibroNove libroNove);
        Task<List<LibroNove>> RegistroDeHoyPorLinea(int idLinea);
        Task<List<LibroNove>> ObtenerLibroNovedadesPorFiltro(int idCentro,DateTime fecha,int idLinea,int tipoNov);
        Task<bool> UpdateRegistros(List<LibroNove> novedades);
        Task<LibroNove>? ObtenerPorIdParada(string idParada);
        Task<LibroNove>? ObtenerLibroPorId(int idRegistro);
        Task<bool> ActualizacionCompleta(int IdlibrNov,LibroNove data);
        Task<bool> ActualizacionEstado(int IdlibrNov,LibroNove data);
        Task<List<LibroNove>> ObtenerNovedadePorLinea(int IdLiena);
        Task<List<LibroNove>> ObtenerLibroNovedadesDelAreaQueCarga(DateTime fecha,int idLinea,int tipoNov,int IdAreaCar);
        Task<(IEnumerable<IGrouping<DateTime, LibroNove>>,int,int,double)> CalcularCumplimiento(DateTime fechaInicion,DateTime fechafinal,string tipo,int idCondicional);
        Task<List<LibroNove>> ObtenerLibroNovedadesPorFiltroEntreFechas(int idCentro,DateTime fechaInicion,DateTime fechaFinal,int idLinea,int tipoNov);
        Task<List<LibroNove>> ObtenerLibroNovedadesDelAreaQueCargaEntreFechas(DateTime fechaInicio,DateTime fechaFinal,int idLinea,int tipoNov,int IdAreaCar);
    }

    public interface IDataPizarra
    {
        Task<bool> InsertarRegistros(List<ReuDium> reunionDia);
    }

    public interface IDataClasifiTPM
    {
        Task<List<ClasifiTpm>> ObtenerClasificacion();
    }

    public class DataClasifiTPM :IDataClasifiTPM
    {
        private readonly DbNeoContext _cotext;

        public DataClasifiTPM(DbNeoContext context)
        {
            this._cotext = context;
        }

        public async Task<List<ClasifiTpm>> ObtenerClasificacion()
        {
            return await this._cotext.ClasifiTpms.Where(c => c.Ctpmestado == true).ToListAsync();
        }
    }

    public interface IDataTiParTP
    {
        Task<TiParTp> ObtenerTipoParadaId(string IdGespline);

        Task<List<TiParTp>> ObtenerTodosTiposNovedad();
    }


    public class DataLibroNov : IDataLibroNov
    {
        private readonly DbNeoContext _cotext;

        public DataLibroNov(DbNeoContext context)
        {
            this._cotext = context;
        }
        public async Task<bool> InsertarRegistro(LibroNove libroNove)
        {
            this._cotext.LibroNoves.Add(libroNove);
            return await _cotext.SaveChangesAsync() > 0;
        }

        public async Task<LibroNove>? ObtenerPorIdParada(string idParada)
        {
            LibroNove data = await this._cotext.LibroNoves.Where(x => x.IdParada == idParada).FirstOrDefaultAsync();
            return data;
        }

        public async Task<bool> ActualizacionCompleta(int IdlibrNov,LibroNove data){
            LibroNove dataNove = await this._cotext.LibroNoves.Where(x=> x.IdlibrNov == IdlibrNov).FirstOrDefaultAsync();
            if(data != null){
                dataNove.IdLinea = data.IdLinea;
                dataNove.IdAreaCar = data.IdAreaCar;
                dataNove.IdEquipo = data.IdEquipo;
                dataNove.IdlibrNov =  data.IdlibrNov;
                dataNove.IdMaquina = data.IdMaquina;
                dataNove.IdParada = data.IdParada;
                dataNove.IdTipoNove = data.IdTipoNove;
                dataNove.Lndiscrepa = data.Lndiscrepa;
                dataNove.Lnfecha = data.Lnfecha;
                dataNove.LnfichaRes = data.LnfichaRes;
                dataNove.Lngrupo = data.Lngrupo;
                dataNove.LnisPizUni = data.LnisPizUni;
                dataNove.Lnobserv = data.Lnobserv;
                dataNove.LntiePerMi = data.LntiePerMi;
                dataNove.Lnturno = data.Lnturno;
                dataNove.IdCtpm = data.IdCtpm;
                dataNove.LnisResu = data.LnisResu;
                return 0 < await _cotext.SaveChangesAsync();
            }
            return false;         
        }

        public async Task<bool> ActualizacionEstado(int IdlibrNov,LibroNove data){
            LibroNove dataNove1 = new LibroNove();
            dataNove1 = await this.ObtenerLibroPorId(IdlibrNov) ?? new LibroNove();
            if(dataNove1 != null){
                dataNove1.LnisResu = data.LnisResu;
                return 0 < await _cotext.SaveChangesAsync();
            }
            return false;         
        }
        public async Task<List<LibroNove>> RegistroDeHoyPorLinea(int idLinea)
        {
            return await this._cotext.LibroNoves.Where(t => t.IdLinea == idLinea && (t.Lnfecha >= DateTime.Today && t.Lnfecha <  DateTime.Today.AddDays(1))).ToListAsync();
        }

        public async Task<List<LibroNove>> ObtenerNovedadePorLinea(int IdLiena){
            return await this._cotext.LibroNoves.Where(l => l.IdLinea == IdLiena).ToListAsync();
        }


        public async Task<bool> UpdateRegistros(List<LibroNove> novedades)
        {
            LibroNove registro;
            foreach (var item in novedades)
            {
                try{
                    
                        registro = await this._cotext.LibroNoves.FirstOrDefaultAsync(x => x.IdlibrNov == item.IdlibrNov);
                        if(registro != null){
                            registro.LnisPizUni =  item.LnisPizUni;
                        }
                        await _cotext.SaveChangesAsync();
                    
                }catch{
                    try{
                        
                            registro = await this._cotext.LibroNoves.FirstOrDefaultAsync(x => x.IdlibrNov == item.IdlibrNov);
                            if(registro != null){
                                registro.LnisPizUni =  item.LnisPizUni;
                            }
                            await _cotext.SaveChangesAsync();
                        
                    }catch{
                        return false;
                    }
                }
            }
            return true;
        }
        public async Task<List<LibroNove>> ObtenerLibroNovedadesPorFiltro(int idCentro,DateTime fecha,int idLinea,int tipoNov)
        {
            if(idCentro == 0){
                if(idLinea == 0 && tipoNov == 0){
                    return await this._cotext.LibroNoves.Where(t => (t.Lnfecha >= fecha && t.Lnfecha < fecha.AddDays(1))).Include(t => t.IdLineaNavigation).ThenInclude(L => L.IdDivisionNavigation).ThenInclude(L => L.IdCentroNavigation).Include(t => t.IdTipoNoveNavigation).AsNoTracking().ToListAsync();
                }else if(idLinea == 0){
                    return await this._cotext.LibroNoves.Where(t => (t.Lnfecha >= fecha && t.Lnfecha < fecha.AddDays(1)) && (t.IdTipoNove == tipoNov)).Include(t => t.IdLineaNavigation).ThenInclude(L => L.IdDivisionNavigation).ThenInclude(L => L.IdCentroNavigation).Include(t => t.IdTipoNoveNavigation).AsNoTracking().ToListAsync();
                }else if(tipoNov == 0){
                    return await this._cotext.LibroNoves.Where(t => (t.Lnfecha >= fecha && t.Lnfecha < fecha.AddDays(1)) && (t.IdLinea == idLinea)).Include(t => t.IdLineaNavigation).ThenInclude(L => L.IdDivisionNavigation).ThenInclude(L => L.IdCentroNavigation).Include(t => t.IdTipoNoveNavigation).AsNoTracking().ToListAsync();
                }else{
                    return await this._cotext.LibroNoves.Where(t => (t.Lnfecha >= fecha && t.Lnfecha < fecha.AddDays(1)) && ((t.IdTipoNove == tipoNov) && (t.IdLinea == idLinea))).Include(t => t.IdLineaNavigation).ThenInclude(L => L.IdDivisionNavigation).ThenInclude(L => L.IdCentroNavigation).Include(t => t.IdTipoNoveNavigation).AsNoTracking().ToListAsync();
                }
            }else{
                if(idLinea == 0 && tipoNov == 0){
                    return await this._cotext.LibroNoves.Where(t => (t.Lnfecha >= fecha && t.Lnfecha < fecha.AddDays(1)) && t.IdLineaNavigation.IdCentro == idCentro).Include(t => t.IdLineaNavigation).ThenInclude(L => L.IdDivisionNavigation).ThenInclude(L => L.IdCentroNavigation).Include(t => t.IdTipoNoveNavigation).AsNoTracking().ToListAsync();
                }else if(idLinea == 0){
                    return await this._cotext.LibroNoves.Where(t => (t.Lnfecha >= fecha && t.Lnfecha < fecha.AddDays(1)) && (t.IdTipoNove == tipoNov) && t.IdLineaNavigation.IdCentro == idCentro).Include(t => t.IdLineaNavigation).ThenInclude(L => L.IdDivisionNavigation).ThenInclude(L => L.IdCentroNavigation).Include(t => t.IdTipoNoveNavigation).AsNoTracking().ToListAsync();
                }else if(tipoNov == 0){
                    return await this._cotext.LibroNoves.Where(t => (t.Lnfecha >= fecha && t.Lnfecha < fecha.AddDays(1)) && (t.IdLinea == idLinea) && t.IdLineaNavigation.IdCentro == idCentro).Include(t => t.IdLineaNavigation).ThenInclude(L => L.IdDivisionNavigation).ThenInclude(L => L.IdCentroNavigation).Include(t => t.IdTipoNoveNavigation).AsNoTracking().ToListAsync();
                }else{
                    return await this._cotext.LibroNoves.Where(t => (t.Lnfecha >= fecha && t.Lnfecha < fecha.AddDays(1)) && ((t.IdTipoNove == tipoNov) && (t.IdLinea == idLinea)) && t.IdLineaNavigation.IdCentro == idCentro).Include(t => t.IdLineaNavigation).ThenInclude(L => L.IdDivisionNavigation).ThenInclude(L => L.IdCentroNavigation).Include(t => t.IdTipoNoveNavigation).AsNoTracking().ToListAsync();
                }
            }
        }

        public async Task<List<LibroNove>> ObtenerLibroNovedadesPorFiltroEntreFechas(int idCentro,DateTime fechaInicion,DateTime fechaFinal,int idLinea,int tipoNov)
        {
            if(idCentro == 0){
                if(idLinea == 0 && tipoNov == 0){
                    return await this._cotext.LibroNoves.Where(t => (t.Lnfecha.Date >= fechaInicion.Date && t.Lnfecha.Date <= fechaFinal.Date)).Include(t => t.IdLineaNavigation).ThenInclude(L => L.IdDivisionNavigation).ThenInclude(L => L.IdCentroNavigation).Include(t => t.IdTipoNoveNavigation).AsNoTracking().ToListAsync();
                }else if(idLinea == 0){
                    return await this._cotext.LibroNoves.Where(t => (t.Lnfecha.Date >= fechaInicion.Date && t.Lnfecha.Date <= fechaFinal.Date) && (t.IdTipoNove == tipoNov)).Include(t => t.IdLineaNavigation).ThenInclude(L => L.IdDivisionNavigation).ThenInclude(L => L.IdCentroNavigation).Include(t => t.IdTipoNoveNavigation).AsNoTracking().ToListAsync();
                }else if(tipoNov == 0){
                    return await this._cotext.LibroNoves.Where(t => (t.Lnfecha.Date >= fechaInicion.Date && t.Lnfecha.Date <= fechaFinal.Date) && (t.IdLinea == idLinea)).Include(t => t.IdLineaNavigation).ThenInclude(L => L.IdDivisionNavigation).ThenInclude(L => L.IdCentroNavigation).Include(t => t.IdTipoNoveNavigation).AsNoTracking().ToListAsync();
                }else{
                    return await this._cotext.LibroNoves.Where(t => (t.Lnfecha.Date >= fechaInicion.Date && t.Lnfecha.Date <= fechaFinal.Date) && ((t.IdTipoNove == tipoNov) && (t.IdLinea == idLinea))).Include(t => t.IdLineaNavigation).ThenInclude(L => L.IdDivisionNavigation).ThenInclude(L => L.IdCentroNavigation).Include(t => t.IdTipoNoveNavigation).AsNoTracking().ToListAsync();
                }
            }else{
                if(idLinea == 0 && tipoNov == 0){
                    return await this._cotext.LibroNoves.Where(t => (t.Lnfecha.Date >= fechaInicion.Date && t.Lnfecha.Date <= fechaFinal.Date) && t.IdLineaNavigation.IdCentro == idCentro).Include(t => t.IdLineaNavigation).ThenInclude(L => L.IdDivisionNavigation).ThenInclude(L => L.IdCentroNavigation).Include(t => t.IdTipoNoveNavigation).AsNoTracking().ToListAsync();
                }else if(idLinea == 0){
                    return await this._cotext.LibroNoves.Where(t => (t.Lnfecha.Date >= fechaInicion.Date && t.Lnfecha.Date <= fechaFinal.Date) && (t.IdTipoNove == tipoNov) && t.IdLineaNavigation.IdCentro == idCentro).Include(t => t.IdLineaNavigation).ThenInclude(L => L.IdDivisionNavigation).ThenInclude(L => L.IdCentroNavigation).Include(t => t.IdTipoNoveNavigation).AsNoTracking().ToListAsync();
                }else if(tipoNov == 0){
                    return await this._cotext.LibroNoves.Where(t => (t.Lnfecha.Date >= fechaInicion.Date && t.Lnfecha.Date <= fechaFinal.Date) && (t.IdLinea == idLinea) && t.IdLineaNavigation.IdCentro == idCentro).Include(t => t.IdLineaNavigation).ThenInclude(L => L.IdDivisionNavigation).ThenInclude(L => L.IdCentroNavigation).Include(t => t.IdTipoNoveNavigation).AsNoTracking().ToListAsync();
                }else{
                    return await this._cotext.LibroNoves.Where(t => (t.Lnfecha.Date >= fechaInicion.Date && t.Lnfecha.Date <= fechaFinal.Date) && ((t.IdTipoNove == tipoNov) && (t.IdLinea == idLinea)) && t.IdLineaNavigation.IdCentro == idCentro).Include(t => t.IdLineaNavigation).ThenInclude(L => L.IdDivisionNavigation).ThenInclude(L => L.IdCentroNavigation).AsNoTracking().Include(t => t.IdTipoNoveNavigation).ToListAsync();
                }
            }
        }
        public async Task<List<LibroNove>> ObtenerLibroNovedadesDelAreaQueCarga(DateTime fecha,int idLinea,int tipoNov,int IdAreaCar)
        {
            if(idLinea == 0 && tipoNov == 0){
                return await this._cotext.LibroNoves.Where(t => (t.Lnfecha >= fecha && t.Lnfecha < fecha.AddDays(1)) && t.IdAreaCar == IdAreaCar).Include(t => t.IdLineaNavigation).ThenInclude(L => L.IdDivisionNavigation).ThenInclude(L => L.IdCentroNavigation).Include(t => t.IdTipoNoveNavigation).AsNoTracking().ToListAsync();
            }else if(idLinea == 0){
                return await this._cotext.LibroNoves.Where(t => (t.Lnfecha >= fecha && t.Lnfecha < fecha.AddDays(1)) && (t.IdTipoNove == tipoNov) && t.IdAreaCar == IdAreaCar).Include(t => t.IdLineaNavigation).ThenInclude(L => L.IdDivisionNavigation).ThenInclude(L => L.IdCentroNavigation).Include(t => t.IdTipoNoveNavigation).AsNoTracking().ToListAsync();
            }else if(tipoNov == 0){
                return await this._cotext.LibroNoves.Where(t => (t.Lnfecha >= fecha && t.Lnfecha < fecha.AddDays(1)) && (t.IdLinea == idLinea) && t.IdAreaCar == IdAreaCar).Include(t => t.IdLineaNavigation).ThenInclude(L => L.IdDivisionNavigation).ThenInclude(L => L.IdCentroNavigation).Include(t => t.IdTipoNoveNavigation).AsNoTracking().ToListAsync();
            }else{
                return await this._cotext.LibroNoves.Where(t => (t.Lnfecha >= fecha && t.Lnfecha < fecha.AddDays(1)) && ((t.IdTipoNove == tipoNov) && (t.IdLinea == idLinea)) && t.IdAreaCar == IdAreaCar).Include(t => t.IdLineaNavigation).ThenInclude(L => L.IdDivisionNavigation).ThenInclude(L => L.IdCentroNavigation).Include(t => t.IdTipoNoveNavigation).AsNoTracking().ToListAsync();
            }
        }

        public async Task<List<LibroNove>> ObtenerLibroNovedadesDelAreaQueCargaEntreFechas(DateTime fechaInicio,DateTime fechaFinal,int idLinea,int tipoNov,int IdAreaCar)
        {
            if(idLinea == 0 && tipoNov == 0){
                return await this._cotext.LibroNoves.Where(t => (t.Lnfecha.Date >= fechaInicio.Date && t.Lnfecha.Date <= fechaFinal.Date) && t.IdAreaCar == IdAreaCar).Include(t => t.IdLineaNavigation).ThenInclude(L => L.IdDivisionNavigation).ThenInclude(L => L.IdCentroNavigation).Include(t => t.IdTipoNoveNavigation).AsNoTracking().ToListAsync();
            }else if(idLinea == 0){
                return await this._cotext.LibroNoves.Where(t => (t.Lnfecha.Date >= fechaInicio.Date && t.Lnfecha.Date <= fechaFinal.Date) && (t.IdTipoNove == tipoNov) && t.IdAreaCar == IdAreaCar).Include(t => t.IdLineaNavigation).ThenInclude(L => L.IdDivisionNavigation).ThenInclude(L => L.IdCentroNavigation).Include(t => t.IdTipoNoveNavigation).AsNoTracking().ToListAsync();
            }else if(tipoNov == 0){
                return await this._cotext.LibroNoves.Where(t => (t.Lnfecha.Date >= fechaInicio.Date && t.Lnfecha.Date <= fechaFinal.Date) && (t.IdLinea == idLinea) && t.IdAreaCar == IdAreaCar).Include(t => t.IdLineaNavigation).ThenInclude(L => L.IdDivisionNavigation).ThenInclude(L => L.IdCentroNavigation).Include(t => t.IdTipoNoveNavigation).AsNoTracking().ToListAsync();
            }else{
                return await this._cotext.LibroNoves.Where(t => (t.Lnfecha.Date >= fechaInicio.Date && t.Lnfecha.Date <= fechaFinal.Date) && ((t.IdTipoNove == tipoNov) && (t.IdLinea == idLinea)) && t.IdAreaCar == IdAreaCar).Include(t => t.IdLineaNavigation).ThenInclude(L => L.IdDivisionNavigation).ThenInclude(L => L.IdCentroNavigation).Include(t => t.IdTipoNoveNavigation).AsNoTracking().ToListAsync();
            }
        }
        public async Task<(IEnumerable<IGrouping<DateTime, LibroNove>>,int,int,double)> CalcularCumplimiento(DateTime fechaInicion,DateTime fechafinal,string tipo,int idCondicional)
        {
            IEnumerable<IGrouping<DateTime, LibroNove>> data;
            List<LibroNove> libroNov = new List<LibroNove>();
            int diasReales = 0;
            double cumplimiento;
            int diasToricos = (fechafinal.Day - fechaInicion.Day) + 1;
            if(tipo == "Centro"){
                libroNov = await this._cotext.LibroNoves.Where(l => l.IdLineaNavigation.IdDivisionNavigation.IdCentro == idCondicional && (l.Lnfecha.Date >= fechaInicion.Date &&  l.Lnfecha.Date <= fechafinal.Date)).Include(t => t.IdLineaNavigation).ToListAsync(); //.Select(l => new LibroNove() {Lnfecha = l.Lnfecha})
            }else if(tipo == "Division"){
                libroNov = await this._cotext.LibroNoves.Where(l => l.IdLineaNavigation.IdDivision == idCondicional && (l.Lnfecha.Date >= fechaInicion.Date &&  l.Lnfecha.Date <= fechafinal.Date)).Include(t => t.IdLineaNavigation).ToListAsync(); //.Select(l => new LibroNove() {Lnfecha = l.Lnfecha})
            }else if(tipo == "Linea"){
                libroNov = await this._cotext.LibroNoves.Where(l => l.IdLineaNavigation.IdLinea == idCondicional && (l.Lnfecha.Date >= fechaInicion.Date &&  l.Lnfecha.Date <= fechafinal.Date)).Include(t => t.IdLineaNavigation).ToListAsync(); //.Select(l => new LibroNove() {Lnfecha = l.Lnfecha})
            }
            data = libroNov.GroupBy(l => l.Lnfecha.Date);
            diasReales = data.Count();
            cumplimiento = (double) diasReales / diasToricos;
            return (data,diasToricos,diasReales,cumplimiento);
        }

        public async Task<LibroNove>? ObtenerLibroPorId(int idRegistro){
            LibroNove data = await this._cotext.LibroNoves.Where(l => l.IdlibrNov == idRegistro).FirstOrDefaultAsync();
            return data;
        }
    }
    public class DataTiParTP : IDataTiParTP
    {
        private readonly DbNeoContext _cotext;

        public DataTiParTP(DbNeoContext context)
        {
            this._cotext = context;
        }
        public async Task<TiParTp> ObtenerTipoParadaId(string IdGespline){
            return await this._cotext.TiParTps.Where(t => t.Tpcodigo == IdGespline).FirstOrDefaultAsync();
        }

        public async Task<List<TiParTp>> ObtenerTodosTiposNovedad(){
            return await this._cotext.TiParTps.Where(t => t.Tpestado == true).ToListAsync();
        }
    }
    public class DataPizarra : IDataPizarra
    {

        private readonly DbNeoContext _cotext;

        public DataPizarra(DbNeoContext context)
        {
            this._cotext = context;
        }
        public async Task<bool> InsertarRegistros(List<ReuDium> reunionDia){
            foreach (var item in reunionDia)
            {
                this._cotext.ReuDia.Add(item);
            }
            return await _cotext.SaveChangesAsync() > 0;
        }
    }
    

    public interface IDataChismoso
    {
        Task<bool> InsertarRegistros(List<CambFec> data,List<CambStat> data2);
    }

     public class DataChismoso : IDataChismoso
    {
        private readonly DbNeoContext _cotext;

        public DataChismoso(DbNeoContext context)
        {
            this._cotext = context;
        }

        public async Task<bool> InsertarRegistros(List<CambFec> data,List<CambStat> data2){
            foreach (var item in data)
            {
                this._cotext.CambFecs.Add(item);
            }
            foreach (var item in data2)
            {
                this._cotext.CambStats.Add(item);
            }
            return await _cotext.SaveChangesAsync() > 0;
        }
    }

}