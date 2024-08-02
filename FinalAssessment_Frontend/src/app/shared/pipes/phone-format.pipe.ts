import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'phoneFormat'
})
export class PhoneFormatPipe implements PipeTransform {

  transform(phone : string): string {
    if (!phone || phone.length !== 10) {
      return phone; 
    }


    // const prefix = '+91 '; 
    const part1 = phone.slice(0, 3);  
    const part2 = phone.slice(3, 6); 
    const part3 = phone.slice(6);    
  
    // return `${prefix}(${part1}) ${part2}-${part3
    return `(${part1}) ${part2}-${part3}`;

  }

}
