﻿namespace ScreenSound.Web.Requests;

public record ArtistaRequestEdit(int Id, string Nome, string Bio)
    : ArtistaRequest(Nome, Bio);

