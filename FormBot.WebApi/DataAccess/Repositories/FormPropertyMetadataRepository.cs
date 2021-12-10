using CliverBot.Console.DataAccess.Entities;
using FormBot.WebApi.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JutsuBot.Elements.DataAccess.Repositories
{
    public class FormPropertyMetadataRepository
    {
        private readonly ApplicationDbContext _context;

        public FormPropertyMetadataRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public FormPropertyMetadata GetFormPropertyMetadata(int formId, string propertyName)
        {
           return _context.FormProperties.SingleOrDefault(p => p.FormId == formId && p.PropertyName == propertyName);
        }

        public async Task<FormPropertyMetadata> ChangePropertyStatus(int formId, string propertyName, PropertyStatus status, object value)
        {
            var property = GetFormPropertyMetadata(formId, propertyName);
            if (property is not null)
            {
                property.PropertyStatus = status;
                property.Value = value;
            }
            else
            {
                FormPropertyMetadata newProperty = new();

                newProperty.PropertyName = propertyName;
                newProperty.PropertyStatus = status;
                newProperty.Value = value;

                property = (await _context.FormProperties.AddAsync(newProperty)).Entity;
            }

            await _context.SaveChangesAsync();
            return property;
        }
    }
}
