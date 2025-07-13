namespace Application.Interfaces;

public interface IBaseRepository<T1, T2>
{
    Task<IQueryable<T1>> GetAllAsync();
    Task<T1?> GetByIdAsync(int id);
    Task<T2?> AddAsync(T1 entity);
    Task<T2?> UpdateAsync(T1 entity);
    Task<T2?> DeleteAsync(T1 entity);
}

//  •	T1 — это модель базы данных, например Car, User, Product.
//	•	T2 — это DTO (Data Transfer Object), то есть то, что возвращается наружу, например CarDTO, UserDTO, ProductDTO.