using Chat.Core.Entities;
using Chat.DataAccess.Contexts;
using Chat.DataAccess.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.DataAccess.Repository.Implementations;

public class MessageRepository : Repository<Message>, IMessageRepository
{
    public MessageRepository(AppDbContexts context) : base(context)
    {
    }
}
