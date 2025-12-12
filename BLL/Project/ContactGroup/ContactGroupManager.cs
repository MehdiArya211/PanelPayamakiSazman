using DTO.Base;
using DTO.DataTable;
using DTO.Project.ContactGroup;
using DTO.WebApi;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BLL.Project.ContactGroup
{
    public class ContactGroupManager : IContactGroupManager
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly HttpClient _client;
        private readonly string _baseUrl;

        public ContactGroupManager(IHttpContextAccessor accessor)
        {
            _httpContext = accessor;
            _client = new HttpClient();
            _baseUrl = "http://87.107.111.44:8010";
        }

        
        private void SetAuth()
        {
            var token = _httpContext.HttpContext?.Session.GetString("AdminToken");

            _client.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrWhiteSpace(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        private JsonSerializerOptions JsonOptions =>
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        /* ==============================
         *          GROUPS
         * ==============================*/

        public DataTableResponseDTO<ContactGroupListDTO> GetGroupsDataTable(DataTableSearchDTO search)
        {
            SetAuth();

            // API: POST /api/consumer/contact-groups/search
            var url = $"{_baseUrl}/api/consumer/contact-groups/search";

            var body = new
            {
                page = (search.start / search.length) + 1,
                pageSize = search.length,
                filters = new List<object>(),
                sortBy = string.IsNullOrWhiteSpace(search.sortColumnName) ? "name" : search.sortColumnName,
                sortDescending = search.sortDirection == "desc"
            };

            var json = JsonSerializer.Serialize(body, JsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = _client.PostAsync(url, content).Result;
            var jsonResult = res.Content.ReadAsStringAsync().Result;

            if (!res.IsSuccessStatusCode)
            {
                return new DataTableResponseDTO<ContactGroupListDTO>
                {
                    draw = search.draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<ContactGroupListDTO>(),
                    AdditionalData = jsonResult
                };
            }

            var apiResponse =
                JsonSerializer.Deserialize<ApiResponsePagedDTO<ContactGroupListDTO>>(jsonResult, JsonOptions);

            var total = apiResponse?.Meta?.TotalCount ?? 0;

            return new DataTableResponseDTO<ContactGroupListDTO>
            {
                draw = search.draw,
                recordsTotal = total,
                recordsFiltered = total,
                data = apiResponse?.Data ?? new List<ContactGroupListDTO>()
            };
        }

        public List<ContactGroupListDTO> GetAllGroups()
        {
            try
            {
                SetAuth();

                // چون GET نداریم، از search با pageSize بزرگ استفاده می‌کنیم
                var url = $"{_baseUrl}/api/consumer/contact-groups/search";

                var body = new
                {
                    page = 1,
                    pageSize = 200,
                    filters = new List<object>(),
                    sortBy = "name",
                    sortDescending = false
                };

                var json = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;
                var jsonResult = res.Content.ReadAsStringAsync().Result;

                if (!res.IsSuccessStatusCode)
                    return new List<ContactGroupListDTO>();

                var apiResponse =
                    JsonSerializer.Deserialize<ApiResponsePagedDTO<ContactGroupListDTO>>(jsonResult, JsonOptions);

                return apiResponse?.Data ?? new List<ContactGroupListDTO>();
            }
            catch
            {
                return new List<ContactGroupListDTO>();
            }
        }

        public ContactGroupEditDTO GetGroupById(string id)
        {
            var all = GetAllGroups();

            var item = all.FirstOrDefault(x => x.Id == id);
            if (item == null) return null;

            return new ContactGroupEditDTO
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description
            };
        }

        public BaseResult CreateGroup(ContactGroupCreateDTO model)
        {
            try
            {
                SetAuth();

                // API: POST /api/consumer/contact-groups
                var url = $"{_baseUrl}/api/consumer/contact-groups";

                var body = new
                {
                    name = model.Name,
                    description = model.Description
                };

                var json = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "گروه با موفقیت ایجاد شد." };

                return new BaseResult
                {
                    Status = false,
                    Message = res.Content.ReadAsStringAsync().Result
                };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }

        public BaseResult UpdateGroup(ContactGroupEditDTO model)
        {
            try
            {
                SetAuth();

                // API: PUT /api/consumer/contact-groups/{groupId}
                var url = $"{_baseUrl}/api/consumer/contact-groups/{model.Id}";

                var body = new
                {
                    name = model.Name,
                    description = model.Description
                };

                var json = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PutAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "گروه با موفقیت ویرایش شد." };

                return new BaseResult
                {
                    Status = false,
                    Message = res.Content.ReadAsStringAsync().Result
                };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }

        public BaseResult DeleteGroup(string id)
        {
            try
            {
                SetAuth();

                // API: DELETE /api/consumer/contact-groups/{groupId}
                var url = $"{_baseUrl}/api/consumer/contact-groups/{id}";

                var res = _client.DeleteAsync(url).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "گروه با موفقیت حذف شد." };

                return new BaseResult
                {
                    Status = false,
                    Message = res.Content.ReadAsStringAsync().Result
                };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }

        /* ==============================
         *          CONTACTS
         * ==============================*/

        public DataTableResponseDTO<ContactDto> GetContactsDataTable(string groupId, DataTableSearchDTO search)
        {
            SetAuth();

            // API: POST /api/consumer/contact-groups/{groupId}/contacts/search
            var url = $"{_baseUrl}/api/consumer/contact-groups/{groupId}/contacts/search";

            var body = new
            {
                page = (search.start / search.length) + 1,
                pageSize = search.length,
                filters = new List<object>(),
                sortBy = string.IsNullOrWhiteSpace(search.sortColumnName) ? "id" : search.sortColumnName,
                sortDescending = search.sortDirection == "desc"
            };

            var json = JsonSerializer.Serialize(body, JsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = _client.PostAsync(url, content).Result;
            var jsonResult = res.Content.ReadAsStringAsync().Result;

            if (!res.IsSuccessStatusCode)
            {
                return new DataTableResponseDTO<ContactDto>
                {
                    draw = search.draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<ContactDto>(),
                    AdditionalData = jsonResult
                };
            }

            var apiResponse =
                JsonSerializer.Deserialize<ApiResponsePagedDTO<ContactDto>>(jsonResult, JsonOptions);

            var total = apiResponse?.Meta?.TotalCount ?? 0;

            return new DataTableResponseDTO<ContactDto>
            {
                draw = search.draw,
                recordsTotal = total,
                recordsFiltered = total,
                data = apiResponse?.Data ?? new List<ContactDto>()
            };
        }

        public BaseResult AddSingleContact(string groupId, ContactInputDTO contact)
        {
            try
            {
                SetAuth();

                // API: POST /api/consumer/contact-groups/{groupId}/contacts
                // body: { contacts: [ {...} ] }
                var url = $"{_baseUrl}/api/consumer/contact-groups/{groupId}/contacts";

                var body = new
                {
                    contacts = new[]
                    {
                        new
                        {
                            phoneNumber = contact.PhoneNumber,
                            firstName = contact.FirstName,
                            lastName = contact.LastName,
                            nationalCode = contact.NationalCode,
                            birthDate = contact.BirthDate,
                            customFields = contact.CustomFields
                        }
                    }
                };

                var json = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "مخاطب با موفقیت اضافه شد." };

                return new BaseResult { Status = false, Message = res.Content.ReadAsStringAsync().Result };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }

        public BaseResult AddContactsByText(string groupId, string numbers)
        {
            try
            {
                SetAuth();

                // API: POST /api/consumer/contact-groups/{groupId}/contacts/text
                var url = $"{_baseUrl}/api/consumer/contact-groups/{groupId}/contacts/text";

                var body = new
                {
                    numbers = numbers
                };

                var json = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "شماره‌ها با موفقیت ثبت شدند." };

                return new BaseResult { Status = false, Message = res.Content.ReadAsStringAsync().Result };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }

        public BaseResult UploadContactsCsv(string groupId, IFormFile file)
        {
            try
            {
                SetAuth();

                // API: POST /api/consumer/contact-groups/{groupId}/contacts/csv (multipart)
                var url = $"{_baseUrl}/api/consumer/contact-groups/{groupId}/contacts/csv";

                using var multipart = new MultipartFormDataContent();

                using var stream = file.OpenReadStream();
                var fileContent = new StreamContent(stream);
                multipart.Add(fileContent, "file", file.FileName);

                var res = _client.PostAsync(url, multipart).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "فایل با موفقیت آپلود و پردازش شد." };

                return new BaseResult { Status = false, Message = res.Content.ReadAsStringAsync().Result };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }

        public BaseResult Deduplicate(string groupId)
        {
            try
            {
                SetAuth();

                // API: POST /api/consumer/contact-groups/{groupId}/deduplicate
                var url = $"{_baseUrl}/api/consumer/contact-groups/{groupId}/deduplicate";

                // بدنه ندارد (طبق داکیومنت)
                var res = _client.PostAsync(url, null).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "تکراری‌ها با موفقیت حذف شدند." };

                return new BaseResult { Status = false, Message = res.Content.ReadAsStringAsync().Result };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }
    }
}
