using AutoMapper;
using ManejoPresupuestos.Models;
using ManejoPresupuestos.Models.ViewModels;

namespace ManejoPresupuestos.Services {
    public class AutoMapperProfile : Profile {
        public AutoMapperProfile() {
            CreateMap<Cuenta, CuentaCreacionViewModel>();
            CreateMap<TransaccionActualizacionViewModel, Transaccion>().ReverseMap();
        }
    }
}