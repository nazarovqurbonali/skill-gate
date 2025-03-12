global using Npgsql;
global using Domain.Enums;
global using Domain.Common;
global using Domain.Entities;
global using Domain.Constants;
global using System.Security.Claims;
global using Microsoft.AspNetCore.Http;
global using Infrastructure.Extensions;
global using Application.Extensions.Http;
global using Microsoft.Extensions.Logging;
global using Application.Contracts.Services;
global using Application.Extensions.Filters;
global using Application.Extensions.Algorithms;
global using Microsoft.Extensions.Configuration;
global using Application.Contracts.Repositories;
global using Microsoft.AspNetCore.Authentication;
global using Application.Extensions.ResultPattern;
global using Application.Contracts.Repositories.Base.Crud;
global using Application.Extensions.DTOs.Identity.Requests;
global using Application.Extensions.DTOs.Identity.Responses;
global using Application.Extensions.DTOs.Identity.Responses;
global using Application.Extensions.Responses.PagedResponse;
global using Infrastructure.ImplementationContract.Repositories.NpgsqlMappers;
global using Infrastructure.ImplementationContract.Repositories.NpgsqlCommands;
global using Infrastructure.ImplementationContract.Repositories.NpgsqlParameters;
