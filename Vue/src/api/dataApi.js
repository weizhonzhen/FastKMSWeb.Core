import http from '@/api/http'

export const tableList = (key,pageId,pageSize) =>http.get(`/data/tableList?key=${key}&pageId=${pageId}&pageSize=${pageSize}`)

export const viewList = (key,pageId,pageSize) =>http.get(`/data/viewList?key=${key}&pageId=${pageId}&pageSize=${pageSize}`)

export const dataList = () =>http.get('/data/dataList')

export const columnList = (key,tabName,isView) =>http.get(`/data/columnList?key=${key}&tableName=${tabName}&isView=${isView}`)

export const tabUpdate = (data) =>http.post('/data/updateTabComments',data, {headers: {'Content-Type': 'application/json'}});

export const colUpdate = (formData) =>http.post('/data/UpdateColComments',formData, {headers: {'Content-Type': 'multipart/form-data'}});