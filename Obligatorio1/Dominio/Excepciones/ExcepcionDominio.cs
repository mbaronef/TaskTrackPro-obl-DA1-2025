﻿namespace Dominio.Excepciones;

public class ExcepcionDominio : Exception
{
    public ExcepcionDominio(string mensaje) : base(mensaje)
    {
    }
}