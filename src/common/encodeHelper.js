export function getFileTypeFromBase64(rawBase64Str) {
  return rawBase64Str.split("/")[1].split(";")[0]
}
export function formatBase64(rawBase64Str) {
//   console.log(`%c#######,RawBase64 is [ ${rawBase64Str} ] `, 'color: red')
  let imgType = rawBase64Str
    .split("/")[1]
    .split(";")[0]
  // .toUpperCase();
//   console.log(`%c#######,type is  [ ${imgType} ]`, 'color: red')

  let result = rawBase64Str
    .split(",")[1]
    .concat("|")
    .concat('.')
    .concat(imgType);
//   console.log(`%c#######,result is [ ${result} ] `, 'color: red')

  return result
}

export default {
  getFileTypeFromBase64,
  formatBase64
}
