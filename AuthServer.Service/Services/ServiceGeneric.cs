﻿using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos;
using System.Linq.Expressions;

namespace AuthServer.Service.Services
{
    public class ServiceGeneric<TEntity, TDto> : IServiceGeneric<TEntity, TDto> where TEntity : class where TDto : class
    {

        private readonly IUnitOfWork _unitOfWork;

        private readonly IGenericRepository<TEntity> _genericRepository;

        public ServiceGeneric(IUnitOfWork unitOfWork, IGenericRepository<TEntity> genericRepository)
        {
            _unitOfWork = unitOfWork;
            _genericRepository = genericRepository;
        }

        public async Task<Response<TDto>> AddAsync(TDto dto)
        {
            var newEntity = ObjectMapper.CreateMapper.Map<TEntity>(dto);

            await _genericRepository.AddAsync(newEntity);

            await _unitOfWork.CommitAsync();

            var newDto = ObjectMapper.CreateMapper.Map<TDto>(newEntity);

            return Response<TDto>.Success(newDto, 200);
        }

        public async Task<Response<IEnumerable<TDto>>> GetAllAsync()
        {
            var products = ObjectMapper.CreateMapper.Map<List<TDto>>(await _genericRepository.GetAllAsync());

            return Response<IEnumerable<TDto>>.Success(products, 200);
        }

        public async Task<Response<TDto>> GetByIdAsync(int id)
        {
            var product = await _genericRepository.GetByIdAsync(id);

            if (product is null) 
            {  
                return Response<TDto>.Fail("Id not found", 404, true);
            }

            return Response<TDto>.Success(ObjectMapper.CreateMapper.Map<TDto>(product), 200);
        }

        public async Task<Response<NoDataDto>> Remove(int id)
        {
            var isExistEntity = await _genericRepository.GetByIdAsync(id);
            if(isExistEntity == null)
            {
                return Response<NoDataDto>.Fail("Id not found",404,true);
            }

            _genericRepository.Remove(isExistEntity);

            await _unitOfWork.CommitAsync();

            return Response<NoDataDto>.Success(204);

        }

        public async Task<Response<NoDataDto>> Update(TDto dto, int id)
        {
            var isExistEntity = await _genericRepository.GetByIdAsync(id);

            if(isExistEntity == null)
            {
                return Response<NoDataDto>.Fail("Id not found", 404, true);
            }

            var updateEntity = ObjectMapper.CreateMapper.Map<TEntity>(dto);

            _genericRepository.Update(updateEntity);

            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Success(204);
        }

        public async Task<Response<IEnumerable<TDto>>> Where(Expression<Func<TEntity, bool>> predicate)
        {
            var list = _genericRepository.Where(predicate);

            return Response<IEnumerable<TDto>>.Success(ObjectMapper.CreateMapper.Map<IEnumerable<TDto>>(await list.ToListAsync()),200);
        }
    }
}
