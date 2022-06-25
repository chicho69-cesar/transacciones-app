﻿namespace ManejoPresupuestos.Models.ViewModels {
    public class IndiceCuentaViewModel {
        public string TipoCuenta { get; set; }
        public IEnumerable<Cuenta> Cuentas { get; set; }

        public decimal Balance => Cuentas
            .Sum(c => c.Balance);
    }
}