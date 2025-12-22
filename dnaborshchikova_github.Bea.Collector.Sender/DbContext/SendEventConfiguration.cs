using dnaborshchikova_github.Bea.Collector.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dnaborshchikova_github.Bea.Collector.Sender.DbContext
{
    public class SendEventConfiguration : IEntityTypeConfiguration<SendEvent>
    {
        public void Configure(EntityTypeBuilder<SendEvent> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(e => e.UserId).IsRequired();
            builder.Property(e => e.EventType).IsRequired();
            builder.Property(e => e.Date).IsRequired();
            builder.ToTable("SendEvents");
        }
    }
}
