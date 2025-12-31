import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'revers'
})
export class ReversPipe implements PipeTransform {

  transform(value: string): string {
    return value.split('').reverse().join('');
  }

}
