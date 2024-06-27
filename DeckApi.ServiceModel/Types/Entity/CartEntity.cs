using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeckApi.ServiceModel.Types.Entity;

public class CartEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    // identity auth uses guid for id
    [Required] 
    public string UserId { get; set; }

    public ICollection<CartItemEntity> Items { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsActive { get; set; }
}