import axiosInstance from '@/api/axiosInstance'

export const tableList = (key,pageId,pageSize) =>axiosInstance.get(`/data/tableList?key=${key}&pageId=${pageId}&pageSize=${pageSize}`)

export const viewList = (key,pageId,pageSize) =>axiosInstance.get(`/data/viewList?key=${key}&pageId=${pageId}&pageSize=${pageSize}`)

export const dataList = () =>axiosInstance.get('/data/dataList')

export const columnList = (key,tabName,isView) =>axiosInstance.get(`/data/columnList?key=${key}&tableName=${tabName}&isView=${isView}`)

export const tabUpdate = (data) =>axiosInstance.post('/data/updateTabComments',data, {headers: {'Content-Type': 'application/json'}});

export const colUpdate = (formData) =>axiosInstance.post('/data/UpdateColComments',formData, {headers: {'Content-Type': 'multipart/form-data'}});